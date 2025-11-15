# MdViewer

Lightweight WPF Markdown viewer control for .NET 10.

`MdViewer` is a small WPF `UserControl` that converts Markdown to HTML (via `Markdig`) and displays it using the built-in `WebBrowser` control.

## Features

- Render Markdown to styled HTML
- Simple `Content` property to set Markdown text
- External `http/https` links open in the default browser
- Temporary HTML is removed when the control is disposed

## Quick start

> [!tip]
>
> You can view a sample app in `tests/MdViewer.SampleApp`.

Steps to add `MdViewer` to your project:

1. Add the namespace reference:

    ```xaml
    xmlns:md="clr-namespace:MdViewer;assembly=MdViewer"
    ```

2. Insert the control where you want to display Markdown (suggested `FontSize`: 16):

    ```xaml
    <md:Markdown x:Name="MdViewer" FontSize="16" />
    ```

    You can also set `FontFamily`.

3. Load Markdown by setting the `Content` property:

    ```csharp
    MdViewer.Content = "# Hello, Markdown!";
    ```

4. Call `Dispose()` when the window is closed to clean up the temporary file:

    ```csharp
    MdViewer.Dispose();
    ```
## Screenshot

![screenshot](https://raw.githubusercontent.com/masterLazy/MdViewer/refs/heads/main/img/screenshot.webp)

## Dependencies & license

- Uses `Markdig` for conversion (https://github.com/xoofx/markdig)
- Licensed under Apache License 2.0
