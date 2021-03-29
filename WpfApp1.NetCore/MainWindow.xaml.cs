using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace WpfApp1.NetCore
{
	public class ListItem
	{
		public string Title { get; set; }
		public ListItem()
		{

		}
		public ListItem(string title)
		{
			Title = title;
		}
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
		: Window
	{
		private ObservableCollection<ListItem> _items { get; set; }
		private uint _index = 1;

		public MainWindow()
		{
			DataContext = this;
			InitializeComponent();
			_items = new ObservableCollection<ListItem>
			{
				new ListItem("Item 1") ,
				new ListItem("Item 2")
			};
			listBox.ItemsSource = _items;
		}

		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);
			var height = double.IsNaN(Height)
					 ? ActualHeight
					 : Height;
			Top = SystemParameters.FullPrimaryScreenHeight - height + 22;
			Left = SystemParameters.FullPrimaryScreenWidth - Width;
			listBox.MaxHeight = SystemParameters.FullPrimaryScreenHeight - 69;
		}

		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			var count = _items.Count;
			for (var i = count; i <= count + 20; i++)
			{
				_items.Add(new ListItem($"Item{i + 1}"));
			}
			var screenHeight = SystemParameters.FullPrimaryScreenHeight;
			if (Top + ActualHeight > screenHeight - Top)
			{
				listBox.Height = double.NaN;
				//Top -= ActualHeight - 300;
				Top = screenHeight - listBox.ActualHeight - (433 * _index);
				_index *= 2;
				//Height = screenHeight - listBox.Height;
				if (Top < 0)
					Top = 0;
				if (ActualHeight > screenHeight - 22)
					listBox.Height = screenHeight - BtnBack.Height - BtnAdd.Height + 22;
			}

		}
	}
}
