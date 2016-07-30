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
}

type Quiz struct {
	Corpus []*Question
}

func (q *Quiz) AddHandlers() {
	http.HandleFunc("/quiz", func(w http.ResponseWriter, r *http.Request) {
		// How many do we want?
		n := 1
		vals := r.URL.Query()
		if t, err := strconv.Atoi(vals.Get("n")); err == nil {
			n = t
		}

		// Pick that many questions.
		qs := make([]*Question, 0, n)
		for i := 0; i < n; i++ {
			u := q.Corpus[rand.Intn(len(q.Corpus))]
			qs = append(qs, u)
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
