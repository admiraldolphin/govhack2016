package quiz

// Question is information needed to ask a multi-choice question and judge answers.
type Question struct {
	Clue    string   `json:"clue"`
	Choices []string `json:"choices"`
	Answer  string   `json:"answer"`
}
