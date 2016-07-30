package portrait

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"math/rand"
	"net/http"
	"path/filepath"
	"strings"

	"github.com/admiraldolphin/govhack2016/server/quiz"
)

const source = "National Portrait Gallery"

// Database holds all the info on portraits.
type Database struct {
	BasePath  string
	Portraits []Portrait
}

type Portrait struct {
	File  string
	Title string
}

func (p *Portrait) ImagePath() string {
	return fmt.Sprintf("/npg/img/%s", p.File)
}

// Load reads the National Portrait Gallery database from files.
func Load(base string) (*Database, error) {
	b, err := ioutil.ReadFile(filepath.Join(base, "portraitau-20160705.json"))
	if err != nil {
		return nil, err
	}
	var pm map[string]string
	if err := json.Unmarshal(b, &pm); err != nil {
		return nil, err
	}
	p := make([]Portrait, 0, len(pm))
	for f, t := range pm {
		p = append(p, Portrait{
			File:  f,
			Title: t,
		})
	}
	log.Printf("Loaded %d portrait entries", len(p))
	return &Database{
		BasePath:  base,
		Portraits: p,
	}, nil
}

func (db *Database) AddHandlers() {
	http.HandleFunc("/npg/img/", func(w http.ResponseWriter, r *http.Request) {
		f := filepath.Join(db.BasePath, "portraits", strings.TrimPrefix(r.URL.Path, "/npg/img/"))
		http.ServeFile(w, r, f)
	})
}

func (db *Database) MakeQuestions(n int) []*quiz.Question {
	corpus := make([]*quiz.Question, 0, n)
	for len(corpus) < n {
		ans := db.Portraits[rand.Intn(len(db.Portraits))]
		c := []string{ans.ImagePath()}
		for len(c) < 4 {
			i := db.Portraits[rand.Intn(len(db.Portraits))]
			if i == ans {
				continue
			}
			c = append(c, i.ImagePath())
		}

		corpus = append(corpus, &quiz.Question{
			Clue:    ans.Title,
			Choices: c,
			Answer:  c[0],
			Source:  source,
		})
	}
	return corpus
}
