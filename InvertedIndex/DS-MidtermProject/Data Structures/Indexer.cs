using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DS_MidtermProject {
    class Indexer {

        public int method;
        public long memory;
        public string path;
        public bool isReady = false;

        BST bst;
        TST tst;
        Trie trie;
        Hash<string, IndexNode> hash;
        IndexNode result;
        MainWindow main;

        string temp, sentence;
        char[] charsSep = { ' ', ',', ';', '.' };

        int wordsToSearchCount;
        List<string> wordsToSearch;
        List<IndexNode> searchResult, words = new List<IndexNode>();
        List<string> stopWords = new List<string>() { "about", "above", "according", "across", "after", "afterwards", "again", "against", "albeit", "all", "nt", "almost", "alone", "along", "already", "also", "although", "always", "am", "among", "amongst", "an", "and", "another", "any", "anybody", "anyhow", "anyone", "anything", "anyway", "anywhere", "apart", "are", "around", "as", "at", "av", "be", "became", "because", "become", "becomes", "becoming", "been", "before", "beforehand", "behind", "being", "below", "beside", "besides", "between", "beyond", "both", "but", "by", "can", "cannot", "canst", "certain", "cf", "choose", "contrariwise", "cos", "could", "cu", "day", "do", "does", "doesn't", "doing", "dost", "doth", "double", "down", "dual", "during", "each", "either", "else", "elsewhere", "enough", "et", "etc", "even", "ever", "every", "everybody", "everyone", "everything", "everywhere", "except", "excepted", "excepting", "exception", "exclude", "excluding", "exclusive", "far", "farther", "farthest", "few", "ff", "first", "for", "formerly", "forth", "forward", "from", "front", "further", "furthermore", "furthest", "get", "go", "had", "halves", "hardly", "has", "hast", "hath", "have", "he", "hence", "henceforth", "her", "here", "hereabouts", "hereafter", "hereby", "herein", "hereto", "hereupon", "hers", "herself", "him", "himself", "hindmost", "his", "hither", "hitherto", "how", "however", "howsoever", "i", "ie", "if", "in", "inasmuch", "inc", "include", "included", "including", "indeed", "indoors", "inside", "insomuch", "instead", "into", "inward", "inwards", "is", "it", "its", "itself", "just", "kind", "kg", "km", "last", "latter", "latterly", "less", "lest", "let", "like", "little", "ltd", "many", "may", "maybe", "me", "meantime", "meanwhile", "might", "moreover", "most", "mostly", "more", "mr", "mrs", "ms", "much", "must", "my", "myself", "namely", "need", "neither", "never", "nevertheless", "next", "no", "nobody", "none", "nonetheless", "noone", "nope", "nor", "not", "nothing", "notwithstanding", "now", "nowadays", "nowhere", "of", "off", "often", "ok", "on", "once", "one", "only", "onto", "or", "other", "others", "otherwise", "ought", "our", "ours", "ourselves", "out", "outside", "over", "own", "per", "perhaps", "plenty", "provide", "quite", "rather", "really", "round", "said", "sake", "same", "sang", "save", "saw", "see", "seeing", "seem", "seemed", "seeming", "seems", "seen", "seldom", "selves", "sent", "several", "shalt", "she", "should", "shown", "sideways", "since", "slept", "slew", "slung", "slunk", "smote", "so", "some", "somebody", "somehow", "someone", "something", "sometime", "sometimes", "somewhat", "somewhere", "spake", "spat", "spoke", "spoken", "sprang", "sprung", "stave", "staves", "still", "such", "supposing", "than", "that", "the", "thee", "their", "them", "themselves", "then", "thence", "thenceforth", "there", "thereabout", "thereabouts", "thereafter", "thereby", "therefore", "therein", "thereof", "thereon", "thereto", "thereupon", "these", "they", "this", "those", "thou", "though", "thrice", "through", "throughout", "thru", "thus", "thy", "thyself", "till", "to", "together", "too", "toward", "towards", "ugh", "unable", "under", "underneath", "unless", "unlike", "until", "up", "upon", "upward", "upwards", "us", "use", "used", "using", "very", "via", "vs", "want", "was", "we", "week", "well", "were", "what", "whatever", "whatsoever", "when", "whence", "whenever", "whensoever", "where", "whereabouts", "whereafter", "whereas", "whereat", "whereby", "wherefore", "wherefrom", "wherein", "whereinto", "whereof", "whereon", "wheresoever", "whereto", "whereunto", "whereupon", "wherever", "wherewith", "whether", "whew", "which", "whichever", "whichsoever", "while", "whilst", "whither", "who", "whoa", "whoever", "whole", "whom", "whomever", "whomsoever", "whose", "whosoever", "why", "will", "wilt", "with", "within", "without", "worse", "worst", "would", "wow", "ye", "yet", "year", "yippee", "you", "your", "yours", "yourself", "yourselves" };

        public Indexer(MainWindow main) {
            this.main = main;
        }

        public int Count {
            get { return words.Count; }
        }

        public void build() {
            switch (method) {
                case 0:
                    memory = GC.GetTotalMemory(true);
                    bst = new BST(words);
                    memory = GC.GetTotalMemory(true) - memory;
                    break;
                case 1:
                    memory = GC.GetTotalMemory(true);
                    tst = new TST(words);
                    memory = GC.GetTotalMemory(true) - memory;
                    break;
                case 2:
                    memory = GC.GetTotalMemory(true);
                    trie = new Trie(words);
                    memory = GC.GetTotalMemory(true) - memory;
                    break;
                case 3:
                    hash = new Hash<string, IndexNode>();
                    memory = GC.GetTotalMemory(true);
                    foreach (IndexNode t in words) {
                        hash.put(t.word, t);
                    }
                    memory = GC.GetTotalMemory(true) - memory;
                    break;
            }
            isReady = true;
        }

        public async void search(string word, bool isWord = true) { // another thread for searching
            await Task.Run(() => {
                int depth = -1;
                switch (method) {
                    case 0:
                        result = bst.search(word);
                        depth = bst.depth();
                        break;
                    case 1:
                        result = tst.search(word);
                        depth = tst.depth();
                        break;
                    case 2:
                        result = trie.search(word);
                        depth = trie.depth();
                        break;
                    case 3:
                        result = hash.search(word);
                        depth = -1;

                        break;
                    default:
                        result = null;
                        break;
                }

                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() => {
                    if (isWord) {
                        main.showSearchResult(result, depth);
                    } else {
                        addSearchResult(result);
                        main.changeProgressBar(wordsToSearchCount - wordsToSearch.Count, wordsToSearchCount);
                        wordsToSearch.Remove(word);
                        if (wordsToSearch.Count > 0) {
                            search(wordsToSearch[0], false);
                        } else {
                            evalSearchResult(depth);
                        }
                    }
                }));
            });
        }

        public void searchWords(string sentence) {
            this.sentence = sentence;
            wordsToSearch = new List<string>();
            searchResult = new List<IndexNode>();
            sentence = sentence.Trim().ToLower().Replace("\n", " ");

            string[] found = sentence.Split(charsSep, StringSplitOptions.RemoveEmptyEntries);

            foreach (string currentWord in found) {

                temp = Regex.Replace(currentWord, "[^a-zA-Z]", ""); // only a-z

                if (!stopWords.Contains(temp) && temp.Length > 1) {
                    if (!wordsToSearch.Contains(temp)) {
                        wordsToSearch.Add(temp);
                    }
                }
            }
            wordsToSearchCount = wordsToSearch.Count;
            main.openProgressBar();

            if (wordsToSearch.Count > 0) {
                search(wordsToSearch[0], false);
            } else {
                main.showSentenceSearchResult("", null);
                main.openProgressBar(false);
            }
        }

        void addSearchResult(IndexNode word) {
            searchResult.Add(word);
        }

        void evalSearchResult(int depth = -1) {
            bool isResult = false;
            List<string> result = new List<string>();

            foreach (IndexNode t in searchResult) {
                if (t == null) {
                    result = null;
                    break;
                }
                if (isResult) {
                    result = intersection(result, t.files);
                    if (result.Count == 0) {
                        break;
                    }
                } else {
                    result = t.files;
                    isResult = true;
                }
            }
            main.showSentenceSearchResult(sentence, result, depth);
            main.openProgressBar(false);
        }

        List<string> intersection(List<string> s1, List<string> s2) {

            List<string> ret = new List<string>();

            foreach (string t in s2) {
                if (s1.Contains(t)) {
                    ret.Add(t);
                }
            }
            return ret;
        }

        public void add(string filename) {
            evaluate(filename);
        }

        public void update(string filename) {
            for (int i = words.Count - 1; i > -1; i--) {
                words[i].files.Remove(filename);
                if (words[i].files.Count == 0) words.Remove(words[i]);
            }
            evaluate(filename);
            build();
        }

        void evaluate(string filename) {
            string input = File.ReadAllText(string.Format("{0}\\{1}", path, filename));
            input = input.Trim().ToLower().Replace("\n", " ");

            string[] found = input.Split(charsSep, StringSplitOptions.RemoveEmptyEntries);

            foreach (string currentWord in found) {

                temp = Regex.Replace(currentWord, "[^a-zA-Z]", ""); // only a-z

                if (!stopWords.Contains(temp) && temp.Length > 1) {
                    addFileToNode(temp, filename);
                }
            }
        }

        void addFileToNode(string word, string filename) {
            foreach (IndexNode s in words) {
                if (word == s.word) {
                    s.addFile(filename);
                    return;
                }
            }
            words.Add(new IndexNode(temp, filename));
        }

        public List<IndexNode> getWords() {
            return words;
        }
    }
}
