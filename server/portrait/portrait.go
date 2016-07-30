package portrait

import (
	"encoding/json"
	"io/ioutil"
	"log"
	"path/filepath"
)

// Database holds all the info on portraits.
type Database struct {
	Portraits map[string]string // Image file -> Name.
}

// Load reads the National Portrait Gallery database from files.
func Load(base string) (*Database, error) {
	b, err := ioutil.ReadFile(filepath.Join(base, "portraitau-20160705.json"))
	if err != nil {
		return nil, err
	}
	var pm map[string]string
	if err := json.Unmarshal(b, &pm); err != nil {
		return nil, err
	}
	log.Printf("Loaded %d portrait entries", len(pm))
	return &Database{Portraits: pm}, nil
}
