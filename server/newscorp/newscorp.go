package newscorp

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

const source = "Newscorp"

type Item struct {
	Title    string   `json:"-"`
	File     string   `json:"image"`
	Keywords []string `json:"keywords"`
}

func (i *Item) ImagePath() string {
	return fmt.Sprintf("/newscorp/img/%s", i.File)
}

type Database struct {
	BasePath string
	Items    []*Item
}

func Load(base string) (*Database, error) {
	b, err := ioutil.ReadFile(filepath.Join(base, "newscorp.json"))
	if err != nil {
		return nil, err
	}
	var im map[string]*Item
	if err := json.Unmarshal(b, &im); err != nil {
		return nil, err
	}
	is := make([]*Item, 0, len(im))
	for t, i := range im {
		i.Title = t
		is = append(is, i)
	}
	log.Printf("Loaded %d Newscorp items", len(is))
	return &Database{
		BasePath: base,
		Items:    is,
	}, nil
}

func (db *Database) AddHandlers() {
	http.HandleFunc("/newscorp/img/", func(w http.ResponseWriter, r *http.Request) {
		log.Printf("%s %s", r.Method, r.URL)
		f := filepath.Join(db.BasePath, strings.TrimPrefix(r.URL.Path, "/newscorp/img/"))
		http.ServeFile(w, r, f)
	})
}

func (db *Database) MakeQuestion() *quiz.Question {
	ans := db.Items[rand.Intn(len(db.Items))]
	c := []string{ans.ImagePath()}
	for len(c) < 4 {
		i := db.Items[rand.Intn(len(db.Items))]
		if i == ans {
			continue
		}
		c = append(c, i.ImagePath())
	}

	return &quiz.Question{
		Clue:    ans.Title,
		Choices: c,
		Answer:  c[0],
		Source:  source,
	}
}
