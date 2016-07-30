package main

import (
	crand "crypto/rand"
	"encoding/binary"
	"flag"
	"fmt"
	"log"
	"math/rand"
	"net/http"

	"github.com/admiraldolphin/govhack2016/server/abc"
	"github.com/admiraldolphin/govhack2016/server/linc"
	"github.com/admiraldolphin/govhack2016/server/newscorp"
	"github.com/admiraldolphin/govhack2016/server/portrait"
	"github.com/admiraldolphin/govhack2016/server/quiz"
)

var (
	abcBase      = flag.String("abc_base", "", "Base path for ABC articles files")
	npgBase      = flag.String("npg_base", "", "Base path for National Portrait Gallery files")
	lincBase     = flag.String("linc_base", "", "Base path for LINC Tasmania files")
	newscorpBase = flag.String("newscorp_base", "", "Base path for Newscorp files")
	port         = flag.Int("port", 8080, "Serving port")
	minItems     = flag.Int("min_items", 5, "Minimum items in a subject to not combine the subject for questions")
)

func main() {
	flag.Parse()

	// Seed the PRNG.
	b := make([]byte, 8)
	if _, err := crand.Read(b); err != nil {
		log.Fatalf("Reading entropy: %v", err)
	}
	rand.Seed(int64(binary.BigEndian.Uint64(b)))

	q := quiz.Quiz{}

	if *abcBase != "" {
		abcDB, err := abc.Load(*abcBase, *minItems)
		if err != nil {
			log.Fatalf("Loading ABC articles database: %v", err)
		}
		abcDB.AddHandlers() // For debugging, try browsing from /abc/subjects
		q.Sources = append(q.Sources, quiz.Source{MakeQuestion: abcDB.MakeQuestion, Ratio: 10})
	}

	if *npgBase != "" {
		npgDB, err := portrait.Load(*npgBase)
		if err != nil {
			log.Fatalf("Loading National Portrait Gallery database: %v", err)
		}
		npgDB.AddHandlers()
		q.Sources = append(q.Sources, quiz.Source{MakeQuestion: npgDB.MakeQuestion, Ratio: 1})
	}

	if *lincBase != "" {
		lincDB, err := linc.Load(*lincBase)
		if err != nil {
			log.Fatalf("Loading LINC Tasmania database: %v", err)
		}
		lincDB.AddHandlers()
		q.Sources = append(q.Sources, quiz.Source{MakeQuestion: lincDB.MakeQuestion, Ratio: 1})
	}

	if *newscorpBase != "" {
		ndb, err := newscorp.Load(*newscorpBase)
		if err != nil {
			log.Fatalf("Loading Newscorp database: %v", err)
		}
		ndb.AddHandlers()
		q.Sources = append(q.Sources, quiz.Source{MakeQuestion: ndb.MakeQuestion, Ratio: 1})
	}

	q.AddHandlers()

	log.Fatal(http.ListenAndServe(fmt.Sprintf(":%d", *port), nil))
}
