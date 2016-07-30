package abc

import (
	"math/rand"

	"github.com/admiraldolphin/govhack2016/server/quiz"
)

const source = "ABC"

func (db *Database) MakeQuestions(n int) []*quiz.Question {
	corpus := make([]*quiz.Question, 0, n)
	for len(corpus) < n {
		// Pick a subject.
		subj := db.Subjects[rand.Intn(len(db.Subjects))]

		// Pick an item to be the answer.
		items := db.BySubject[subj]
		ans := items[rand.Intn(len(items))]
		c := []string{ans.RandomImage()}

		// Pick some other choices.
		for len(c) < 4 {
			i := items[rand.Intn(len(items))]
			if i == ans || len(i.Images) == 0 {
				continue
			}
			c = append(c, i.RandomImage())
		}

		// Make a question
		q := &quiz.Question{
			Clue:    ans.Title,
			Answer:  c[0],
			Choices: c,
			Source:  source,
		}
		corpus = append(corpus, q)
	}
	return corpus
}
