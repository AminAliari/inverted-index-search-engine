using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_MidtermProject {

    public class HashElement<K, V> {
        public K key;
        public V value;

        public HashElement(K key, V value) {
            this.key = key;
            this.value = value;
        }
    }

    public class Hash<K, V> {

        const int TABLE_SIZE = 512;

        HashElement<K, V> temp;
        List<List<HashElement<K, V>>> table;

        public Hash() {
            table = new List<List<HashElement<K, V>>>(TABLE_SIZE);
            for (int i = 0; i < TABLE_SIZE; i++) {
                table.Add(new List<HashElement<K, V>>());
            }
        }

        int getHash(K key) {
            return Math.Abs(key.GetHashCode() % TABLE_SIZE);
        }

        public HashElement<K, V> get(K key) {
            int hash = getHash(key);
            foreach (HashElement<K, V> t in table[hash]) {
                if (t.key.Equals(key)) {
                    return t;
                }
            }
            return null;
        }

        public void put(K key, V value) {
            int hash = getHash(key);
            switch (contains(key, value, hash)) {
                case 0:
                    table[hash].Add(new HashElement<K, V>(key, value));
                    break;

                case 1:
                    get(key).value = value;
                    break;
            }

        }

        public V search(K word) {
            temp = get(word);
            if (temp != null) {
                return temp.value;
            }
            return default(V);
        }

        int contains(K key, V value, int hash) {
            foreach (HashElement<K, V> t in table[hash]) {
                if (t.key.Equals(key) && t.value.Equals(value)) {
                    return 2;
                } else if (t.key.Equals(key)) {
                    return 1;
                }
            }
            return 0;
        }
    }
}
