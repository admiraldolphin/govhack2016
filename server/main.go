package main

import (
	"flag"
	"fmt"
	"log"
	"net/http"

	"github.com/admiraldolphin/govhack2016/server/abc"
	"github.com/admiraldolphin/govhack2016/server/quiz"
)

var (
	articlesBase = flag.String("b", "", "Base path for articles files")
	port         = flag.Int("port", 8080, "Serving port")
	minItems     = flag.Int("min_items", 5, "Minimum items in a subject to make questions from")
)

func main() {
	flag.Parse()
	db, err := abc.Load(*articlesBase, *minItems)
	if err != nil {
		log.Fatalf("Loading ABC articles database: %v", err)
	}
	db.AddHandlers() // Try browsing from at /abc/subjects

	q := quiz.Quiz{
		Corpus: db.MakeQuestions(50),
	}
	q.AddHandlers()

	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%d", *port), nil))
}
