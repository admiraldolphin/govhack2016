package abc

import (
	"fmt"
	"math/rand"

	"github.com/admiraldolphin/govhack2016/server/quiz"
)

func (db *Database) MakeQuestions(n int) []*quiz.Question {
	corpus := make([]*quiz.Question, 0, n)
	for i := 0; i < n; i++ {
		// Pick a subject.
		subj := db.Subjects[rand.Intn(len(db.Subjects))]

		// Pick an item to be the answer.
		items := db.BySubject[subj]
		ans := items[rand.Intn(len(items))]
		// TODO: use other images
		c := []string{fmt.Sprintf("/abc/img/%s/%s", ans.ID, ans.Images[0])}

		// Pick some other choices.
		for len(c) < 4 {
			i := items[rand.Intn(len(items))]
			if i == ans || len(i.Images) == 0 {
				continue
			}
			c = append(c, fmt.Sprintf("/abc/img/%s/%s", i.ID, i.Images[0]))
		}

		// Make a question
		q := &quiz.Question{
			Clue:    ans.Title,
			Answer:  c[0],
			Choices: c,
		}
		corpus = append(corpus, q)
	}
	return corpus
}
