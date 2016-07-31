package images

import (
	"bytes"
	"image"
	"image/color"
	"image/draw"
	"image/jpeg"
	_ "image/png"
	"io"
)

func ApplyBorder(src io.Reader, thickness float64, col color.Color) ([]byte, error) {
	img, _, err := image.Decode(src)
	if err != nil {
		return nil, err
	}
	b := img.Bounds()
	sz := b.Size()
	sx, sy := float64(sz.X)*thickness, float64(sz.Y)*thickness
	// Pick the smaller
	w := int(sx)
	if sy < sx {
		w = int(sy)
	}

	// Make a border
	bdr := image.NewRGBA(b)
	for y := b.Min.Y; y < b.Min.Y+w; y++ {
		for x := b.Min.X; x < b.Max.X; x++ {
			bdr.Set(x, y, col)
		}
	}
	for y := b.Max.Y - w; y < b.Max.Y; y++ {
		for x := b.Min.X; x < b.Max.X; x++ {
			bdr.Set(x, y, col)
		}
	}
	for x := b.Min.X; x < b.Min.X+w; x++ {
		for y := b.Min.Y; y < b.Max.Y; y++ {
			bdr.Set(x, y, col)
		}
	}
	for x := b.Max.X - w; x < b.Max.X; x++ {
		for y := b.Min.Y; y < b.Max.Y; y++ {
			bdr.Set(x, y, col)
		}
	}

	// Composit a new image
	t := image.NewRGBA(b)
	draw.Draw(t, b, img, image.ZP, draw.Over)
	draw.Draw(t, b, bdr, image.ZP, draw.Over)

	// Encode out
	dst := new(bytes.Buffer)
	if err := jpeg.Encode(dst, t, nil); err != nil {
		return nil, err
	}
	return dst.Bytes(), nil
}
