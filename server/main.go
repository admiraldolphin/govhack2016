package main

import (
	"flag"
	"fmt"
	"html/template"
	"log"
	"net/http"
	"path/filepath"
	"strings"

	"github.com/admiraldolphin/govhack2016/server/abc"
)

var (
	articlesBase = flag.String("b", "", "Base path for articles files")
	port         = flag.Int("port", 8080, "Serving port")

	tmplItem = template.Must(template.New("item").Parse(
		`<h1>{{.Title}}</h1><pre>{{ .PrettyJSON }}</pre>{{$id := .ID}}{{ range $i := .Images }}<img src="/img/{{$id}}/{{$i}}" />{{end}}`))
	tmplSubjects = template.Must(template.New("subjects").Parse(
		`<ul>{{ range $k, $v := . }}<li><a href="/subject/{{$k}}">{{$k}} ({{len $v}} items)</a></li>{{ end }}</ul>`))
	tmplSubject = template.Must(template.New("subject").Parse(
		`<ul>{{ range $i := . }}<li><a href="/{{$i.ID}}">{{$i.Title}}</a></li>{{end}}</ul>`))
)

func main() {
	flag.Parse()

	db, err := abc.Load(*articlesBase)
	if err != nil {
		log.Fatalf("Loading ABC articles database: %v", err)
	}

	http.HandleFunc("/img/", func(w http.ResponseWriter, r *http.Request) {
		f := filepath.Join(*articlesBase, strings.TrimPrefix(r.URL.Path, "/img/"))
		http.ServeFile(w, r, f)
	})
	http.HandleFunc("/subject/", func(w http.ResponseWriter, r *http.Request) {
		h := w.Header()
		s := strings.TrimPrefix(r.URL.Path, "/subject/")
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
	http.HandleFunc("/subjects", func(w http.ResponseWriter, r *http.Request) {
		h := w.Header()
		h.Set("Content-Type", "text/html")
		if err := tmplSubjects.Execute(w, db.BySubject); err != nil {
			log.Printf("Executing tmplSubjects: %v", err)
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
		if err := tmplItem.Execute(w, i); err != nil {
			log.Printf("Executing tmplItem: %v", err)
		}
	})

	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%d", *port), nil))
}
