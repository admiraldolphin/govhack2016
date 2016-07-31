package portrait

import (
	"encoding/json"
	"fmt"
	"image/color"
	"io/ioutil"
	"log"
	"math/rand"
	"net/http"
	"path/filepath"
	"strings"

	"github.com/admiraldolphin/govhack2016/server/images"
	"github.com/admiraldolphin/govhack2016/server/quiz"
)

const source = "National Portrait Gallery"

var colour = color.RGBA{0xff, 0x22, 0x11, 0xff}

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
		if t == "Self portrait" {
			// A bit ambiguous given we haven't bothered with artist.
			continue
		}
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
		log.Printf("%s %s", r.Method, r.URL)
		f := filepath.Join(db.BasePath, "portraits", strings.TrimPrefix(r.URL.Path, "/npg/img/"))
		images.ServeBorderedImage(w, r, f, 0.05, colour)
		//http.ServeFile(w, r, f)
	})
}

func (db *Database) MakeQuestion() *quiz.Question {
	ans := db.Portraits[rand.Intn(len(db.Portraits))]
	c := []string{ans.ImagePath()}
	for len(c) < 4 {
		i := db.Portraits[rand.Intn(len(db.Portraits))]
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
		Colour:  images.Hex(colour),
	}
}
