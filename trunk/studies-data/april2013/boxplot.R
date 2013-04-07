cmp <- read.csv("pre-post-tests.csv", sep=";", dec=".")
boxplot(lex ~ TEST, data = cmp, ylab = "Lexical Accuracy" )
boxplot(gram ~ TEST, data = cmp, ylab = "Grammatical accuracy")
boxplot(writing ~ TEST, data = cmp, ylab = "Writing skills")
