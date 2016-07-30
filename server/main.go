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
)

func main() {
	flag.Parse()
	db, err := abc.Load(*articlesBase)
	if err != nil {
		log.Fatalf("Loading ABC articles database: %v", err)
	}
	db.AddHandlers()

	q := quiz.Quiz{
		Corpus: []*quiz.Question{
			{
				Clue:    "Clue 1",
				Choices: []string{"A", "B", "C", "D"},
				Answer:  "A",
			},
			{
				Clue:    "Clue 2",
				Choices: []string{"A", "B", "C", "D"},
				Answer:  "B",
			},
		},
	}
	q.AddHandlers()

	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%d", *port), nil))
}
