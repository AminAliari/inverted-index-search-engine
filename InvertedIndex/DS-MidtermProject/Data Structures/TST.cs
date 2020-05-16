using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_MidtermProject {

    class TSTNode {
        public IndexNode indexData;
        public char data;
        public bool isEnd = false;
        public TSTNode left, middle, right;

        public TSTNode(char data) {
            this.data = data;
        }
    }

    public class TST {

        TSTNode root;
        List<string> al; // for print method

        public TST(List<IndexNode> s) {
            s.Sort((s1, s2) => s1.CompareTo(s2));
            insert(s, 0, s.Count - 1);
        }

        void insert(List<IndexNode> s, int start, int end) {
            if (start > end) return;
            int mid = start + (end - start) / 2;
            insert(s[mid]);
            insert(s, start, mid - 1);
            insert(s, mid + 1, end);
        }

        void insert(IndexNode node) {
            root = insert(root, node, 0);
        }

        TSTNode insert(TSTNode r, IndexNode node, int ptr) {
            if (r == null) {
                r = new TSTNode(node.word[ptr]);
                r.indexData = node;
            }
            if (node.word[ptr] < r.data) {
                r.left = insert(r.left, node, ptr);
            } else if (node.word[ptr] > r.data) {
                r.right = insert(r.right, node, ptr);
            } else {
                if (ptr + 1 < node.word.Length) {
                    r.middle = insert(r.middle, node, ptr + 1);
                } else {
                    r.isEnd = true;
                }
            }
            return r;
        }

        public IndexNode search(string word) {
            return search(root, word, 0);
        }

        IndexNode search(TSTNode r, string word, int ptr) {
            if (r == null) return null;

            if (word[ptr] < r.data) return search(r.left, word, ptr);
            if (word[ptr] > r.data) return search(r.right, word, ptr);

            if (r.isEnd && ptr == word.Length - 1) return r.indexData;
            if (ptr == word.Length - 1) {
                return null;
            } else {
                return search(r.middle, word, ptr + 1);
            }
        }

        public void print() {
            al = new List<string>();
            traverse(root, "");
            foreach (string t in al) {
                Console.Write(t + ", ");
            }
            Console.WriteLine();
        }

        void traverse(TSTNode r, string str) {
            if (r != null) {
                traverse(r.left, str);

                str = str + r.data;
                if (r.isEnd) al.Add(str);

                traverse(r.middle, str);
                str = str.Substring(0, str.Length - 1);

                traverse(r.right, str);
            }
        }

        public int depth() {
            return depth(root);
        }

        int depth(TSTNode t) {
            if (t == null) return 0;
            return Math.Max(Math.Max(depth(t.left), depth(t.middle)),depth(t.right)) + 1;
        }
    }
}
