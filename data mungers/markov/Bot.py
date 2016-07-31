import markovify

with open("corpus.txt") as f:
	text = f.read()

model = markovify.NewlineText(text)

with open("fakenews.txt",'w') as outfile:
    for i in range(1000):
        outfile.writelines(model.make_sentence()+"\n")
