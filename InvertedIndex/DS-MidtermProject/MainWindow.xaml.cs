using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace DS_MidtermProject {
    public partial class MainWindow : Window {

        string lastInput;
        bool isOutput = false; // is there content on screen
        const string nullPathText = "directory path";
        const string defaultCommandText = "command line";

        int lastMethod = 0;
        Indexer indexer;
        Stopwatch timer;
        TextBlock outputItemTb;
        ListBoxItem outputItem;
        System.Windows.Forms.Timer errorTimer = new System.Windows.Forms.Timer();
        List<string> filesNames = new List<string>();
        InputList<string> cList = new InputList<string>();
        SolidColorBrush errorColor = new SolidColorBrush(Color.FromRgb(244, 67, 54));
        SolidColorBrush notifyColor = new SolidColorBrush(Color.FromRgb(15, 174, 80));

        public MainWindow() {
            indexer = new Indexer(this);
            timer = new Stopwatch();
            InitializeComponent();
            errorTimer.Interval = 2000;
            errorTimer.Tick += OnErrorEnd;
        }

        private void build(object sender, RoutedEventArgs e) {
            if (filesNames.Count > 0) {
                try {
                    indexer.build();
                    showNotification(string.Format("operation was successful. [{0}]", ((ComboBoxItem)comboBox.SelectedItem).Content));
                    lastMethod = indexer.method;
                } catch {
                    showError("there was an issue in building index.");
                }
            } else {
                showError("no file found to build.");
            }
        }

        private void commendTb_GotFocus(object sender, RoutedEventArgs e) {
            if (commendTb.Text.Equals(defaultCommandText)) commendTb.Text = "";
        }

        private void commendTb_LostFocus(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(commendTb.Text)) {
                commendTb.Text = defaultCommandText;
            }
        }

        private void browseFolder(object sender, RoutedEventArgs e) {

            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                pathTb.Text = fbd.SelectedPath;
            }
        }

        void evalCommand(string input) {
            List<string> tokens = input.Trim().ToLower().Split(' ').ToList<string>();
            for (int i = tokens.Count - 1; i > -1; i--) {
                tokens[i] = tokens[i].Trim();
                if (string.IsNullOrEmpty(tokens[i])) {
                    tokens.Remove(tokens[i]);
                }
            }
            switch (tokens[0]) {
                case "add":
                    if (tokens.Count > 2) {
                        showError("command not found.");
                        return;
                    }

                    if (indexer.path.Equals(nullPathText)) {
                        showError("set directory path first.");
                        return;
                    }

                    if (tokens.Count < 2 || string.IsNullOrWhiteSpace(tokens[1]) || tokens[1] == ".txt") {
                        showError("enter a file name.");
                        return;
                    }

                    if (!tokens[1].Contains(".txt")) {
                        tokens[1] += ".txt";
                    }

                    if (filesNames.Contains(tokens[1])) {
                        showError("already exists, you may want to update.");
                    } else if (!File.Exists(string.Format("{0}\\{1}", indexer.path, tokens[1]))) {
                        showError("document not found.");
                    } else {
                        filesNames.Add(tokens[1]);
                        indexer.add(tokens[1]);
                        showNotification(string.Format("{0} successfully added.", tokens[1]));
                    }
                    break;

                case "addall":
                    DirectoryInfo filesInfo = new DirectoryInfo(indexer.path);
                    foreach (FileInfo f in filesInfo.GetFiles()) {
                        filesNames.Add(f.Name);
                        indexer.add(f.Name);
                    }
                    break;

                case "update":
                    if (tokens.Count > 2) {
                        showError("command not found.");
                        return;
                    }

                    if (indexer.path.Equals(nullPathText)) {
                        showError("set directory path first.");
                        return;
                    }

                    if (tokens.Count < 2 || string.IsNullOrWhiteSpace(tokens[1]) || tokens[1] == ".txt") {
                        showError("enter a file name.");
                        return;
                    }

                    if (!tokens[1].Contains(".txt")) {
                        tokens[1] += ".txt";
                    }

                    if (filesNames.Contains(tokens[1])) {
                        indexer.update(tokens[1]);
                        showNotification(string.Format("{0} successfully updated.", tokens[1]));
                    } else {
                        showError("document not found.");
                    }
                    break;

                case "del":
                    if (tokens.Count > 2) {
                        showError("command not found.");
                        return;
                    }

                    if (indexer.path.Equals(nullPathText)) {
                        showError("set directory path first.");
                        return;
                    }

                    if (tokens.Count < 2 || string.IsNullOrWhiteSpace(tokens[1]) || tokens[1] == ".txt") {
                        showError("enter a file name.");
                        return;
                    }

                    if (!tokens[1].Contains(".txt")) {
                        tokens[1] += ".txt";
                    }

                    if (!filesNames.Contains(tokens[1])) {
                        showError("document not found.");
                    } else {
                        filesNames.Remove(tokens[1]);
                        showNotification(string.Format("{0} successfully removed from lists.", tokens[1]));
                    }
                    break;

                case "list":
                    if (tokens.Count > 2) {
                        showError("command not found.");
                        return;
                    }

                    if (indexer.path.Equals(nullPathText)) {
                        showError("set directory path first.");
                        return;
                    }

                    if (tokens.Count < 2) {
                        showError("enter an argument. example: \"list -f\"");
                        return;
                    }

                    switch (tokens[1]) {
                        case "-w":
                            if (indexer.Count > 0) {
                                StringBuilder sb = new StringBuilder();

                                foreach (IndexNode t in indexer.getWords()) {
                                    sb.Clear();
                                    sb.Append(t.word);
                                    sb.Append(" -> ");
                                    foreach (string name in t.files) {
                                        sb.Append(name.Replace(".txt", ""));
                                        sb.Append(", ");
                                    }
                                    sb.Remove(sb.Length - 2, 2);
                                    print(sb);
                                }
                                print(string.Format("Number of words = {0}", indexer.Count));
                            } else {
                                showError("no word found.");
                            }
                            break;

                        case "-l":
                            if (filesNames.Count > 0) {
                                StringBuilder sb = new StringBuilder();
                                foreach (string t in filesNames) {
                                    sb.Append(t.Replace(".txt", ""));
                                    sb.Append(", ");
                                }
                                sb.Remove(sb.Length - 2, 2);
                                print(sb);
                                print(string.Format("Number of all docs = {0}", filesNames.Count));
                            } else {
                                showError("no file found.");
                            }
                            break;

                        case "-f":
                            IEnumerable<string> files = Directory.EnumerateFiles(indexer.path, "*.txt").Select(System.IO.Path.GetFileName);

                            if (files.Count<string>() > 0) {
                                StringBuilder sb = new StringBuilder();
                                foreach (string t in files) {
                                    sb.Append(t.Replace(".txt", ""));
                                    sb.Append(", ");
                                }
                                sb.Remove(sb.Length - 2, 2);
                                print(sb);
                                print(string.Format("Number of all docs = {0}", files.Count<string>()));
                            } else {
                                showError("no file found.");
                            }
                            break;
                        default:
                            showError("command not found.");
                            break;
                    }
                    break;

                case "search":

                    if (!indexer.isReady) {
                        showError("build your inverted index first.");
                        return;
                    }

                    if (tokens.Count < 2) {
                        showError("enter an argument. example: search -w \"apple\"");
                        return;
                    }

                    if (tokens.Count < 3) {
                        showError("enter an word. example: search -w \"apple\"");
                        return;
                    }

                    if (!tokens[2].StartsWith("\"") || !input.EndsWith("\"")) {
                        showError("follow this format: search -w \"apple\"");
                        return;
                    }

                    switch (tokens[1]) {
                        case "-w":
                            if (tokens.Count > 3) {
                                showError("command not found.");
                                return;
                            }
                            if (indexer.method.Equals(lastMethod)) {
                                timer.Start();
                                indexer.search(tokens[2].Replace("\"", ""));
                            } else {
                                showError("method has been changed since last build. build again.");
                            }

                            break;

                        case "-s":
                            if (indexer.method.Equals(lastMethod)) {
                                input = input.Substring(input.IndexOf('"')).Replace("\"", "");
                                timer.Start();
                                indexer.searchWords(input);
                            } else {
                                showError("method has been changed since last build. build again.");
                            }
                            break;
                        default:
                            showError("command not found.");
                            break;
                    }
                    break;

                case "cls":
                case "clear":
                    clearScreen();
                    break;

                default:
                    showError("command not found.");
                    break;
            }
        }

        public void showSearchResult(IndexNode result, int depth = -1) {
            timer.Stop();
            if (result != null && result.files.Count > 0) {
                StringBuilder sb = new StringBuilder();
                print(string.Format("[{0}] Appears in:", result.word));
                foreach (string name in result.files) {
                    sb.Append(name.Replace(".txt", ""));
                    sb.Append(", ");
                }

                sb.Remove(sb.Length - 2, 2);
                print(sb);
                if (depth != -1) {
                    print(string.Format("Tree Height: {0}", depth));
                }
                print(string.Format("Hash Used Memory: {0} byte", indexer.memory));
                print(string.Format("Elapsed Time: {0} ms", timer.ElapsedMilliseconds));
                timer.Reset();
            } else {
                showError("no occurance found."); print(string.Format("Tree Height: {0}", depth));
            }
        }

        public void showSentenceSearchResult(string sentence, List<string> result, int depth = -1) {
            if (sentence == null || result == null) { showError("no intersection of all words found."); return; }
            if (result.Count > 0) {
                StringBuilder sb = new StringBuilder();
                print(string.Format("[{0}] Appears in:", sentence));

                foreach (string name in result) {
                    sb.Append(name.Replace(".txt", ""));
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);
                print(sb);
                if (depth != -1) {
                    print(string.Format("Tree Height: {0}", depth));
                }
                print(string.Format("Hash Used Memory: {0} byte", indexer.memory));
                print(string.Format("Elapsed Time: {0} ms", timer.ElapsedMilliseconds));
                timer.Reset();
            } else {
                showError("no intersection of all words found.");
            }
        }

        private void commendTb_KeyUp(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Enter:
                    if (string.IsNullOrEmpty(commendTb.Text)) return;
                    hideError();
                    cList.add(commendTb.Text);
                    evalCommand(commendTb.Text);
                    commendTb.Text = "";
                    break;
                case Key.Up:
                    commendTb.Text = cList.up();
                    if (string.IsNullOrEmpty(commendTb.Text)) { commendTb.Text = lastInput; } else { lastInput = commendTb.Text; }
                    commendTb.CaretIndex = commendTb.Text.Length;
                    break;
                case Key.Down:
                    lastInput = commendTb.Text;
                    commendTb.Text = cList.down();
                    if (string.IsNullOrEmpty(commendTb.Text)) { commendTb.Text = lastInput; } else { lastInput = commendTb.Text; }
                    commendTb.CaretIndex = commendTb.Text.Length;
                    break;
            }
        }

        public void openProgressBar(bool open = true) {
            progressView.IsOpen = open;
        }

        public void changeProgressBar(float value, float total) {
            progressBar.Value = (value / total) * 100;
        }

        void showError(string error) {
            errorTb.Text = string.Format("{0}", error);
            errorRect.Fill = errorColor;
            errorBox.Visibility = Visibility.Visible;
            errorTimer.Stop();
            errorTimer.Start();
        }

        void showNotification(object output) {
            errorTb.Text = string.Format("result: {0}", output.ToString());
            errorRect.Fill = notifyColor;
            errorBox.Visibility = Visibility.Visible;
            errorTimer.Stop();
            errorTimer.Start();
        }

        private void OnErrorEnd(object sender, EventArgs e) {
            hideError();
        }

        void hideError() {
            errorBox.Visibility = Visibility.Hidden;
            errorTimer.Stop();
        }

        private void print(object output) {
            if (isOutput) {
                outputItem = new ListBoxItem();
                outputItemTb = new TextBlock();
                outputItemTb.TextWrapping = TextWrapping.Wrap;
                outputItemTb.Text = string.Format("> {0}", output);
                outputItem.Content = outputItemTb;
                outputList.Items.Add(outputItem);
                outputList.SelectedIndex = outputList.Items.Count - 1;
                outputList.ScrollIntoView(outputList.SelectedItem);
            } else {
                outputItem = (ListBoxItem)outputList.Items[0];
                outputItemTb = (TextBlock)outputItem.Content;
                outputItemTb.Text = string.Format("> {0}", output);
                isOutput = true;
            }
        }

        void clearScreen() {
            if (isOutput) {
                outputList.Items.Clear();
                print("output screen");
                isOutput = false;
            }
        }

        private void setMethod(object sender, SelectionChangedEventArgs e) {
            Int32.TryParse(((ComboBoxItem)comboBox.SelectedItem).Tag.ToString(), out indexer.method);
        }

        private void comboBox_DropDownOpened(object sender, EventArgs e) {
            comboRect.Visibility = Visibility.Visible;
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e) {
            comboRect.Visibility = Visibility.Hidden;
        }

        private void setPath(object sender, TextChangedEventArgs e) {
            indexer.path = pathTb.Text;
        }
    }
}
