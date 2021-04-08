using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace WpfApp1.NetCore
{
	public class ListItem
	{
		public string Title { get; set; }
		public int ItemHeight { get; } = 22;
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
		private struct APPBARDATA
		{
			public int cbSize;
			public IntPtr hWnd;
			public int uCallbackMessage;
			public int uEdge;
			public RECT rc;
			public IntPtr lParam;
		}

		private struct RECT
		{
			public int Left, Top, Right, Bottom;
		}

		private const int _abmGetTaskbarPos = 5;
		private ObservableCollection<ListItem> _items { get; set; }
		private readonly Thickness _backButtonMargin;
		private readonly Thickness _addButtonMargin;

		[DllImport("shell32.dll")]
		private static extern IntPtr SHAppBarMessage(int msg, ref APPBARDATA data);

		private int GetTaskBarHeight()
		{
			var data = new APPBARDATA();
			data.cbSize = Marshal.SizeOf(data);
			var ptr = SHAppBarMessage(_abmGetTaskbarPos, ref data);
			Marshal.FreeHGlobal(ptr);
			return data.rc.Bottom - data.rc.Top; // height
										  //return data.rc.Right - data.rc.Right; // width
		}

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
			_backButtonMargin = BtnBack.Margin;
			_addButtonMargin = BtnAdd.Margin;
		}

		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);
			var screenHeight = SystemParameters.FullPrimaryScreenHeight;
			var height = double.IsNaN(Height)
					 ? ActualHeight
					 : Height;
			var itemHeight = 22;
			var listBoxTotalHeight = _items.Count * itemHeight;
			var otherHeightAndMargin = BtnBack.Height
								+ _backButtonMargin.Top
								+ _backButtonMargin.Bottom
								+ BtnAdd.Height
								+ _addButtonMargin.Top
								+ _addButtonMargin.Bottom;
			var taskbarHeight = GetTaskBarHeight();
			Top = screenHeight - listBoxTotalHeight - otherHeightAndMargin + 10;
			Left = SystemParameters.FullPrimaryScreenWidth - Width;
			listBox.MaxHeight = screenHeight - taskbarHeight;
			MaxHeight = SystemParameters.WorkArea.Height;
			_items.CollectionChanged += Items_CollectionChanged;
		}

		private void Items_CollectionChanged(
			object sender,
			NotifyCollectionChangedEventArgs e
		)
		{
			//var itemHeight = 22;
			//var listBoxTotalHeight = _items.Count * itemHeight;
			//var taskbarHeight = GetTaskBarHeight();
			//var screenHeight = SystemParameters.FullPrimaryScreenHeight;
			//var otherHeightAndMargin = BtnBack.Height
			//					+ _backButtonMargin.Top
			//					+ _backButtonMargin.Bottom
			//					+ BtnAdd.Height
			//					+ _addButtonMargin.Top
			//					+ _addButtonMargin.Bottom;
			//var actualHeight = listBoxTotalHeight + otherHeightAndMargin;
			//if (Top + actualHeight > screenHeight - taskbarHeight)
			//{
			//	Top = screenHeight - actualHeight - taskbarHeight;
			//	if (Top + ActualHeight > screenHeight - taskbarHeight)
			//		Top = screenHeight - ActualHeight - taskbarHeight;
			//	if (actualHeight > screenHeight)
			//	{
			//		var height = screenHeight - taskbarHeight;
			//		listBox.Height = height;
			//		//Top = screenHeight - actualHeight - taskbarHeight - 12;
			//	}
			//	if (Top < 0)
			//		Top = 0;
			//}
		}

		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			var itemsToAdd = 10;
			var count = _items.Count;
			for (var i = count; i < count + itemsToAdd; i++)
			{
				_items.Add(new ListItem($"Item {i + 1}"));
			}
			var taskbarHeight = GetTaskBarHeight();
			var screenHeight = SystemParameters.FullPrimaryScreenHeight;
			var buttonHeights = BtnBack.Height + BtnAdd.Height;
			var buttonMargins = _backButtonMargin.Top
						   + _backButtonMargin.Bottom
						   + _addButtonMargin.Top
						   + _addButtonMargin.Bottom;
			Top = screenHeight - ActualHeight - 3;
			if (itemsToAdd > 1)
				if (Top + ActualHeight + buttonHeights + buttonMargins > screenHeight - taskbarHeight)
					Top = screenHeight 
					    - ActualHeight 
					    - (taskbarHeight * (itemsToAdd - 1)) 
					    + ((itemsToAdd - 2 <= 0 ? 0 : itemsToAdd - 2) * 3);

			if (ActualHeight > screenHeight)
			{
				var height = screenHeight - taskbarHeight - 15;
				listBox.MaxHeight = height;
			}
			if (Top < 0)
				Top = 0;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			_items.CollectionChanged -= Items_CollectionChanged;
		}
	}
}
