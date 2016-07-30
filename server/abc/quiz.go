package abc

import (
	"math/rand"

	"github.com/admiraldolphin/govhack2016/server/quiz"
)

const source = "ABC"

func (db *Database) MakeQuestion() *quiz.Question {
	// Pick a subject.
	subj := db.Subjects[rand.Intn(len(db.Subjects))]

	// Pick an item to be the answer.
	items := db.BySubject[subj]
	ans := items[rand.Intn(len(items))]
	c := []string{ans.RandomImage()}

	// Pick some other choices.
pickChoices:
	for len(c) < 4 {
		i := items[rand.Intn(len(items))]
		if i == ans || len(i.Images) == 0 {
			continue
		}
		im := i.RandomImage()
		for _, k := range c {
			if im == k {
				continue pickChoices
			}
		}
		c = append(c, i.RandomImage())
	}

	// Make a question
	return &quiz.Question{
		Clue:    ans.Title,
		Answer:  c[0],
		Choices: c,
		Source:  source,
	}
}
