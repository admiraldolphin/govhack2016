package csiro

import (
	"encoding/csv"
	"fmt"
	"image/color"
	"io"
	"log"
	"math/rand"
	"net/http"
	"os"
	"path/filepath"
	"strings"

	"github.com/admiraldolphin/govhack2016/server/images"
	"github.com/admiraldolphin/govhack2016/server/quiz"
)

const source = "CSIRO Science Image"

var colour = color.RGBA{0x77, 0x11, 0xff, 0xff}

type Item struct {
	Title string
	File  string
}

func (i *Item) ImagePath() string {
	return fmt.Sprintf("/csiro/img/%s", i.File)
}

type Database struct {
	BasePath string
	Items    []*Item
}

func (db *Database) AddHandlers() {
	http.HandleFunc("/csiro/img/", func(w http.ResponseWriter, r *http.Request) {
		log.Printf("%s %s", r.Method, r.URL)
		f := filepath.Join(db.BasePath, "Image", strings.TrimPrefix(r.URL.Path, "/csiro/img/"))
		//http.ServeFile(w, r, f)
		images.ServeBorderedImage(w, r, f, 0.05, colour)
	})
}

func Load(base string) (*Database, error) {
	f, err := os.Open(filepath.Join(base, "CSIRO_SI_C.unix.csv"))
	if err != nil {
		return nil, err
	}
	defer f.Close()

	r := csv.NewReader(f)
	r.FieldsPerRecord = 4
	var is []*Item
	for {
		rec, err := r.Read()
		if err == io.EOF {
			break
		}
		if err != nil {
			return nil, err
		}
		is = append(is, &Item{
			File:  rec[1],
			Title: rec[2],
		})
	}
	log.Printf("Loaded %d CSIRO Science Image items", len(is))
	return &Database{
		BasePath: base,
		Items:    is,
	}, nil
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
