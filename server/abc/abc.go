package abc

import (
	"encoding/json"
	"fmt"
	"html/template"
	"io/ioutil"
	"log"
	"net/http"
	"path/filepath"
	"strings"
)

var (
	tmplItem = template.Must(template.New("item").Parse(
		`<h1>{{.Title}}</h1><pre>{{ .PrettyJSON }}</pre>{{$id := .ID}}{{ range $i := .Images }}<img src="/abc/img/{{$id}}/{{$i}}" />{{end}}`))
	tmplSubjects = template.Must(template.New("subjects").Parse(
		`<ul>{{ range $k, $v := . }}<li><a href="/abc/subject/{{$k}}">{{$k}} ({{len $v}} items)</a></li>{{ end }}</ul>`))
	tmplSubject = template.Must(template.New("subject").Parse(
		`<ul>{{ range $i := . }}<li><a href="/abc/{{$i.ID}}">{{$i.Title}}</a></li>{{end}}</ul>`))
)

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

func (i *Item) PrettyJSON() string {
	b, err := json.MarshalIndent(i, "", "    ")
	if err != nil {
		log.Printf("Error marshalling some pretty JSON: %v", err)
	}
	return string(b)
}

// Database holds all the summaries in memory organsied in various ways.
type Database struct {
	BasePath  string
	ByID      map[string]*Item
	BySubject map[string][]*Item
}

func (db *Database) AddHandlers() {
	http.HandleFunc("/abc/img/", func(w http.ResponseWriter, r *http.Request) {
		f := filepath.Join(db.BasePath, strings.TrimPrefix(r.URL.Path, "/abc/img/"))
		http.ServeFile(w, r, f)
	})
	http.HandleFunc("/abc/subject/", func(w http.ResponseWriter, r *http.Request) {
		h := w.Header()
		s := strings.TrimPrefix(r.URL.Path, "/abc/subject/")
		i, ok := db.BySubject[s]
		if !ok {
			w.WriteHeader(http.StatusNotFound)
			fmt.Fprintf(w, "404 Not Found")
			return
		}
		h.Set("Content-Type", "text/html")
		if err := tmplSubject.Execute(w, i); err != nil {
			log.Printf("Executing tmplSubject: %v", err)
		}
	})
	http.HandleFunc("/abc/subjects", func(w http.ResponseWriter, r *http.Request) {
		h := w.Header()
		h.Set("Content-Type", "text/html")
		if err := tmplSubjects.Execute(w, db.BySubject); err != nil {
			log.Printf("Executing tmplSubjects: %v", err)
		}
	})
	http.HandleFunc("/abc/", func(w http.ResponseWriter, r *http.Request) {
		id := strings.TrimLeft(r.URL.Path, "/abc/")
		i, ok := db.ByID[id]
		if !ok {
			w.WriteHeader(http.StatusNotFound)
			fmt.Fprintf(w, "404 Not Found")
			return
		}
		if err := tmplItem.Execute(w, i); err != nil {
			log.Printf("Executing tmplItem: %v", err)
		}
	})
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

// Load loads the ABC articles database at a base path.
func Load(base string) (*Database, error) {
	sd, err := filepath.Glob(filepath.Join(base, "*"))
	if err != nil {
		return nil, err
	}

	db := &Database{
		BasePath:  base,
		ByID:      make(map[string]*Item),
		BySubject: make(map[string][]*Item),
	}
	for _, s := range sd {
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
