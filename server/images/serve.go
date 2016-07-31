package images

import (
	"bytes"
	"fmt"
	"image/color"
	"net/http"
	"os"
	"path/filepath"
	"strings"
)

func Hex(c color.RGBA) []int {
	return []int{int(c.R), int(c.G), int(c.B)}
}

func ServeBorderedImage(w http.ResponseWriter, req *http.Request, file string, thickness float64, col color.Color) {
	if strings.Contains(file, "..") {
		w.WriteHeader(http.StatusForbidden)
		fmt.Fprintf(w, "403 Forbidden")
		return
	}

	fi, err := os.Stat(file)
	if err != nil {
		w.WriteHeader(http.StatusNotFound)
		fmt.Fprintf(w, "404 Not Found")
		return
	}

	f, err := os.Open(file)
	if err != nil {
		w.WriteHeader(http.StatusNotFound)
		fmt.Fprintf(w, "404 Not Found")
		return
	}
	defer f.Close()

	b, err := ApplyBorder(f, thickness, col)
	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		fmt.Fprintf(w, "500 Internal Server Error")
		return
	}

	rdr := bytes.NewReader(b)
	http.ServeContent(w, req, filepath.Base(file), fi.ModTime(), rdr)
}
