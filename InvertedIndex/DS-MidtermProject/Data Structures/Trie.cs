using System;
using System.Collections.Generic;


namespace DS_MidtermProject {
    internal class TrieNode {


        public char data;
        public IndexNode indexData;
        public List<TrieNode> children;

        public TrieNode(char data = ' ') {
            this.data = data;
            children = new List<TrieNode>();
        }

        public TrieNode getChild(char c, IndexNode node, bool create = false) {
            foreach (var child in children) {
                if (child.data == c) {
                    return child;
                }
            }

            if (create) {
                return createChild(c, node);
            }

            return null;
        }

        public TrieNode createChild(char c, IndexNode node) {
            TrieNode child = new TrieNode(c);
            child.indexData = node;
            children.Add(child);

            return child;
        }
    }

    public class Trie {
        private TrieNode root;

        public Trie(List<IndexNode> words) {
            root = new TrieNode();
            foreach (IndexNode word in words) {
                insert(word);
            }
        }

        void insert(IndexNode node) {
            TrieNode curr = root;

            curr = curr.getChild(node.word[0], node, true);

            for (int i = 1; i < node.word.Length; i++) {
                curr = curr.getChild(node.word[i], node, true);
            }
        }
        public IndexNode search(string word) {
            return search(word, root);
        }

        IndexNode search(string word, TrieNode node) {
            if (string.IsNullOrEmpty(word)) return node.indexData;
            char c = word[0];
            if (c != '.') {
                if (node.getChild(c,null) == null) {
                    return null;
                } else {
                    return search(word.Substring(1), node.getChild(c,null));
                }
            } else {
                foreach (TrieNode child in node.children) {
                    if (search(word.Substring(1), child) != null) {
                        return child.indexData;
                    }
                }
                return null;
            }
        }

        public int depth() {
            return depth(root);
        }

        int depth(TrieNode t) {
            if (t == null) return 0;
            int d = -1;
            foreach (TrieNode c in t.children) {
                d = Math.Max(d, depth(c));
            }
            return d + 1;
        }
    }
}