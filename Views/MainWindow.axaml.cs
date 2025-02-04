using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace gui_conway.Views;

public partial class MainWindow : Window
{
    private const int GridRows = 15;
    private const int GridColumns = 15;
    private UniformGrid Grid;

    private Button stateButton;

    public MainWindow()
    {
        InitializeComponent();
        stateButton = new Button { Name = "State", Content = "Start", Width = 200, Height = 100 };
        stateButton.Click += (s, e) => GameOfLifeState(e);

        var clearButton = new Button { Name = "Clear", Content = "Clear", Width = 200, Height = 100 };
        clearButton.Click += (c, e) => ClearGame(clearButton, e);

        var buttonPanel = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        buttonPanel.Children.AddRange(new Button[] { stateButton, clearButton });

        Grid = new UniformGrid
        {
            Margin = new Avalonia.Thickness(100),
            Width = 600,
            Height = 600,
            Rows = GridRows,
            Columns = GridColumns,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        for (var i = 0; i < GridRows; ++i)
        {
            for (var j = 0; j < GridColumns; ++j)
            {
                var checkBox = new CheckBox
                {
                    Content = "",
                    Width = 50,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                };

                checkBox.RenderTransform = new ScaleTransform
                {
                    ScaleX = 1.5,
                    ScaleY = 1.5
                };
                Grid.Children.Add(checkBox);
            }
        }

        var panel = new Panel();
        panel.Children.Add(buttonPanel);
        panel.Children.Add(Grid);
        Content = panel;

    }

    private void ClearGame(Button clearButton, RoutedEventArgs e)
    {
        stateButton.Content = "Start";

        for (var i = 0; i < GridRows; ++i)
            for (var j = 0; j < GridColumns; ++j)
                GetCell(i, j).IsChecked = false;
    }

    private async Task GameOfLifeState(RoutedEventArgs e)
    {
        if (stateButton.Content.ToString() == "Start")
        {
            stateButton.Content = "Stop";
            await RunGame();
        }
        else stateButton.Content = "Start";

        e.Handled = true;
    }

    private async Task RunGame()
    {
        int[,] directions =
        {
            {-1,1},  {0,1},  {1,1},
            {-1,0},          {1,0},
            {-1,-1}, {0,-1}, {1,-1},
        };

        while (stateButton.Content.ToString() != "Start")
        {
            var nextState = new bool[GridRows, GridColumns];

            for (var i = 0; i < GridRows; ++i)
            {
                for (var j = 0; j < GridColumns; ++j)
                {
                    var liveNeighbours = 0;
                    for (var k = 0; k < 8; ++k)
                    {
                        var newX = directions[k, 0] + i;
                        var newY = directions[k, 1] + j;

                        if (newX >= 0 && newX < GridRows && newY >= 0 && newY < GridColumns)
                        {
                            if ((bool)GetCell(newX, newY).IsChecked)
                                liveNeighbours += 1;
                        }
                    }

                    var cell = GetCell(i, j);
                    if ((bool)cell.IsChecked)
                        nextState[i, j] = (liveNeighbours == 2 || liveNeighbours == 3);
                    else
                        nextState[i, j] = liveNeighbours == 3;
                }
            }

            for (var i = 0; i < GridRows; ++i)
                for (var j = 0; j < GridColumns; ++j)
                    GetCell(i, j).IsChecked = nextState[i, j];

            await Task.Delay(500);
        }
        Debug.WriteLine("Game Stopped!");
    }

    private CheckBox GetCell(int row, int col)
    {
        if (row < 0 || col < 0 || row >= GridRows || col >= GridColumns)
            throw new ArgumentOutOfRangeException("Incorrect Cell index!");

        var index = row * GridRows + col;
        return (CheckBox)Grid.Children[index];
    }

}
