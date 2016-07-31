package linc

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

const source = "LINC Tasmania"

var colour = color.RGBA{0x11, 0xee, 0x44, 0xff}

type Item struct {
	Subject interface{} `json:"subject"` // Should be either string or []string
	Title   string      `json:"title"`
	File    string      `json:"-"`
}

func (i *Item) ImagePath() string {
	return fmt.Sprintf("/linc/img/%s", i.File)
}

type Database struct {
	BasePath string
	Items    []*Item
}

func Load(base string) (*Database, error) {
	b, err := ioutil.ReadFile(filepath.Join(base, "ARCHDIGITISEDclean.json"))
	if err != nil {
		return nil, err
	}
	var im map[string]*Item
	if err := json.Unmarshal(b, &im); err != nil {
		return nil, err
	}
	is := make([]*Item, 0, len(im))
	for f, i := range im {
		i.File = f
		is = append(is, i)
	}
	log.Printf("Loaded %d LINC items", len(is))
	return &Database{
		BasePath: base,
		Items:    is,
	}, nil
}

func (db *Database) AddHandlers() {
	http.HandleFunc("/linc/img/", func(w http.ResponseWriter, r *http.Request) {
		log.Printf("%s %s", r.Method, r.URL)
		f := filepath.Join(db.BasePath, "linc", strings.TrimPrefix(r.URL.Path, "/linc/img/"))
		//http.ServeFile(w, r, f)
		images.ServeBorderedImage(w, r, f, 0.05, colour)
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
		Colour:  images.Hex(colour),
	}
}
