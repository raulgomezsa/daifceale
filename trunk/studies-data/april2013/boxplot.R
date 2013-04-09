cmp <- read.csv("./pre-post-tests.csv", sep=";", dec=",")

pre <- cmp[cmp$TEST == "pre",]
post <- cmp[cmp$TEST == "post",]

grp1 <- cmp[cmp$Group == "1",]
grp2 <- cmp[cmp$Group == "2",]
grp3 <- cmp[cmp$Group == "3",]

boxplot(nlex ~ TEST, data = cmp, xlab="Test 1", ylab = "Lexical accuracy" )
boxplot(ngram ~ TEST, data = cmp, xlab="Test 2", ylab = "Grammatical accuracy")
boxplot(nwriting ~ TEST, data = cmp, xlab="Test 3", ylab = "Writing skills")
boxplot(overall ~ TEST, data = cmp, xlab="Tests 1-3", ylab = "Overall grade")

boxplot(overall ~ Group, data = pre, xlab="Groups", ylab = "Pre-test grading", ylim=range(0.1,0.9))
boxplot(overall ~ Group, data = post, xlab="Groups", ylab = "Post-test grading", ylim=range(0.1,0.9))

boxplot(nlex ~ TEST, data = grp1, xlab="Group 1", ylab = "Lexical accuracy")
boxplot(ngram ~ TEST, data = grp1, xlab="Group 1", ylab = "Grammatical accuracy")
boxplot(nwriting ~ TEST, data = grp1, xlab="Group 1", ylab = "Writing skills")

boxplot(nlex ~ TEST, data = grp2, xlab="Group 2", ylab = "Lexical accuracy")
boxplot(ngram ~ TEST, data = grp2, xlab="Group 2", ylab = "Grammatical accuracy")
boxplot(nwriting ~ TEST, data = grp2, xlab="Group 2", ylab = "Writing skills")

boxplot(nlex ~ TEST, data = grp3, xlab="Group 3", ylab = "Lexical accuracy")
boxplot(ngram ~ TEST, data = grp3, xlab="Group 3", ylab = "Grammatical accuracy")
boxplot(nwriting ~ TEST, data = grp3, xlab="Group 3", ylab = "Writing skills")

