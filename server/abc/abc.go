package abc

import (
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"path/filepath"
)

// Summary is the info from the Summary_nnnnn.json files.
type Summary struct {
	TeaserTextPlain      string   `json:"teaserTextPlain"`
	ShortTeaserTextPlain string   `json:"shortTeaserTextPlain"`
	TeaserTitle          string   `json:"teaserTitle"`
	ShortTeaserTitle     string   `json:"shortTeaserTitle"`
	Title                string   `json:"title"`
	Images               []string `json:"images"`
	Subjects             []string `json:"subjects"`
}

// Item is a Summary plus other info needed for the item.
type Item struct {
	BasePath string
	ID       string
	*Summary
}

// Database holds all the summaries in memory organsied in various ways.
type Database struct {
	ByID      map[string]*Item
	BySubject map[string][]*Item
}

func loadOne(base string) (*Item, error) {
	id := filepath.Base(base)
	fn := filepath.Join(base, fmt.Sprintf("Summary_%s.json", id))
	b, err := ioutil.ReadFile(fn)
	if err != nil {
		return nil, err
	}
	sum := new(Summary)
	if err := json.Unmarshal(b, sum); err != nil {
		return nil, err
	}
	return &Item{
		BasePath: base,
		ID:       id,
		Summary:  sum,
	}, nil
}

// Load loads the ABC articles database at a base path.
func Load(base string) (*Database, error) {
	sd, err := filepath.Glob(filepath.Join(base, "*"))
	if err != nil {
		return nil, err
	}

	db := &Database{
		ByID:      make(map[string]*Item),
		BySubject: make(map[string][]*Item),
	}
	for _, s := range sd {
		i, err := loadOne(s)
		if err != nil {
			log.Printf("Cannot read: %v", err)
			continue
		}
		db.ByID[i.ID] = i
		for _, c := range i.Subjects {
			db.BySubject[c] = append(db.BySubject[c], i)
		}
	}
	log.Printf("Loaded %d items into %d subjects", len(db.ByID), len(db.BySubject))
	return db, nil
}
