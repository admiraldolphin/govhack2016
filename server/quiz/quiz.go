package quiz

import (
	"encoding/json"
	"fmt"
	"log"
	"math/rand"
	"net/http"
	"strconv"
)

// Question is information needed to ask a multi-choice question and judge answers.
type Question struct {
	Clue    string   `json:"clue"`
	Choices []string `json:"choices"`
	Answer  string   `json:"answer"`
	Source  string   `json:"source"`
}

type Quiz struct {
	Sources []Source
}

type Source struct {
	MakeQuestion func() *Question
	Ratio        int
}

func (q *Quiz) AddHandlers() {
	sum := 0
	for _, s := range q.Sources {
		sum += s.Ratio
	}
	http.HandleFunc("/quiz", func(w http.ResponseWriter, r *http.Request) {
		log.Printf("%s %s", r.Method, r.URL)

		// How many do we want?
		n := 1
		vals := r.URL.Query()
		if t, err := strconv.Atoi(vals.Get("n")); err == nil {
			n = t
		}

		// Generate that many questions.
		qs := make([]*Question, 0, n)
		for len(qs) < n {
			sn := rand.Intn(sum)
			for _, s := range q.Sources {
				if sn >= s.Ratio {
					sn -= s.Ratio
					continue
				}
				qs = append(qs, s.MakeQuestion())
				break
			}
		}

		// Serve all the questions at once
		b, err := json.Marshal(qs)
		if err != nil {
			w.WriteHeader(http.StatusInternalServerError)
			fmt.Fprint(w, "500 Internal Server Error")
			return
		}
		h := w.Header()
		h.Set("Content-Length", fmt.Sprintf("%d", len(b)))
		h.Set("Content-Type", "application/json")
		if _, err := w.Write(b); err != nil {
			log.Printf("Serving /quiz: %v", err)
		}

	})
}
