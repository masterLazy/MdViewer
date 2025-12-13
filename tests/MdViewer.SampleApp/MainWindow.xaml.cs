/**
   Copyright 2025 masterLazy

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using System.Windows;

namespace MdViewer.SampleApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            MdViewer.Content = @"# Welcome to MdViewer

And this is some text with **bold**, *italic* and ~~delete-line~~.

## 这是一个中文标题

中文的显示效果如何呢？

Can you see this `inline code block` ?

Is this [LINK](https://www.example.com) available?

> This is a quote.

---

- This is an unordered list.

1. Ordered list is also supported.

| A | B |
|---|---|
|This should be|a table|

## Code block

```markdown
# Code highlighting is available
This is a markdown in **markdown**.
```
```cpp
// C++
#include <iostream>
using namespace std;
int main() {
    cout << ""Hello, world!\n"";
    return 0;
}
```
```html
<!--html-->
<h1>Hello, world!</h1>
```
";
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            MdViewer.Dispose();
        }
    }
}