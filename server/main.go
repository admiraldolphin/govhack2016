package main

import (
	"flag"
	"fmt"
	"log"
	"net/http"

	"github.com/admiraldolphin/govhack2016/server/abc"
	"github.com/admiraldolphin/govhack2016/server/portrait"
	"github.com/admiraldolphin/govhack2016/server/quiz"
)

var (
	abcBase  = flag.String("abc_base", "", "Base path for ABC articles files")
	npgBase  = flag.String("npg_base", "", "Base path for National Portrait Gallery files")
	port     = flag.Int("port", 8080, "Serving port")
	minItems = flag.Int("min_items", 5, "Minimum items in a subject to make questions from")
)

func main() {
	flag.Parse()
	abcDB, err := abc.Load(*abcBase, *minItems)
	if err != nil {
		log.Fatalf("Loading ABC articles database: %v", err)
	}
	abcDB.AddHandlers() // For debugging. Try browsing from at /abc/subjects

	npgDB, err := portrait.Load(*npgBase)
	if err != nil {
		log.Fatalf("Loading National Portrait Gallery database: %v", err)
	}
	npgDB.AddHandlers()

	q := quiz.Quiz{
		Corpus: append(abcDB.MakeQuestions(50), npgDB.MakeQuestions(5)...),
	}
	q.AddHandlers()

	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%d", *port), nil))
}
