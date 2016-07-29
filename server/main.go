package main

import (
	"encoding/json"
	"flag"
	"fmt"
	"html"
	"io/ioutil"
	"log"
	"net/http"
	"path/filepath"
	"strings"
)

var articlesBase = flag.String("b", "", "Base path for articles files")

// Summary is the info from the Summary_nnnnn.json files.
type Summary struct {
	TeaserTextPlain      string   `json:"teaserTextPlain"`
	ShortTeaserTextPlain string   `json:"shortTeaserTextPlain"`
	TeaserTitle          string   `json:"teaserTitle"`
	ShortTeaserTitle     string   `json:"shortTeaserTitle"`
	Title                string   `json:"title"`
	Images               []string `json:"images"`
	Subjects             []string `json:"subjects"`
}

// Item is a Summary plus other info needed for the item.
type Item struct {
	BasePath string
	ID       string
	*Summary
}

// Database holds all the summaries in memory organsied in various ways.
type Database struct {
	ByID      map[string]*Item
	BySubject map[string][]*Item
}

func loadOne(base string) (*Item, error) {
	id := filepath.Base(base)
	fn := filepath.Join(base, fmt.Sprintf("Summary_%s.json", id))
	b, err := ioutil.ReadFile(fn)
	if err != nil {
		return nil, err
	}
	sum := new(Summary)
	if err := json.Unmarshal(b, sum); err != nil {
		return nil, err
	}
	return &Item{
		BasePath: base,
		ID:       id,
		Summary:  sum,
	}, nil
}

func load(base string) (*Database, error) {
	sd, err := filepath.Glob(filepath.Join(base, "*"))
	if err != nil {
		return nil, err
	}

	db := &Database{
		ByID:      make(map[string]*Item),
		BySubject: make(map[string][]*Item),
	}

	for _, s := range sd {
		//log.Printf("Loading from %s", s)
		i, err := loadOne(s)
		if err != nil {
			log.Printf("Cannot read: %v", err)
			continue
		}
		db.ByID[i.ID] = i
		for _, c := range i.Subjects {
			db.BySubject[c] = append(db.BySubject[c], i)
		}
	}
	log.Printf("Loaded %d items into %d subjects", len(db.ByID), len(db.BySubject))
	return db, nil
}

func main() {
	flag.Parse()

	db, err := load(*articlesBase)
	if err != nil {
		log.Fatal(err)
	}

	http.HandleFunc("/subjects", func(w http.ResponseWriter, r *http.Request) {
		h := w.Header()
		h.Set("Content-Type", "text/html")
		for k, v := range db.BySubject {
			fmt.Fprintf(w, "%s: %d items<br>\n", k, len(v))
		}
	})
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		id := strings.TrimLeft(r.URL.Path, "/")
		i, ok := db.ByID[id]
		if !ok {
			w.WriteHeader(http.StatusNotFound)
			fmt.Fprintf(w, "404 Not Found")
			return
		}
		fmt.Fprintf(w, "%s", html.EscapeString(i.Title))
	})
	log.Fatal(http.ListenAndServe(":8080", nil))
}