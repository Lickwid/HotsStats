﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StatsFetcher;
using System.Threading;
using Heroes.ReplayParser;

namespace StatsDisplay
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : HeroesWindow
	{
		public Game game { get; set; }

		public string MyAccount { get; set; }
		public PlayerProfile Me { get; set; }
		public int MyTeam { get; set; }

		//public int mmr1 { get; set; }
		//public int mmr2 { get; set; }

		public MainWindow()
		{
			InitializeComponent();
		}

		public MainWindow(Game game, string myAccount, bool autoClose)
		{
			InitializeComponent();

			this.game = game;
			MyAccount = myAccount;

			Me = game.Players.Where(p => p.BattleTag == MyAccount || p.Name == MyAccount).FirstOrDefault();
			MyTeam = Me?.Team ?? 0;

			// time for some quick ugly hacks
			var team1 = game.Players.Where(p => p.Team == 0).ToList();
			var team2 = game.Players.Where(p => p.Team == 1).ToList();
			if (team1.Contains(Me)) {
				team1.Remove(Me);
				team1.Insert(0, Me);
			}
			if (team2.Contains(Me)) {
				team2.Remove(Me);
				team2.Insert(0, Me);
			}
			Team1.ItemsSource = team1;
			Team2.ItemsSource = team2;
			Team1.ItemTemplate = this.Resources[MyTeam == 0 ? "BlueRow" : "RedRow" ] as DataTemplate;
			Team2.ItemTemplate = this.Resources[MyTeam == 1 ? "BlueRow" : "RedRow" ] as DataTemplate;

			mmr1_label.Content = "Average MMR: " + (int)team1.Average(p => p.Ranks[GameMode.QuickMatch].Mmr);
			mmr2_label.Content = "Average MMR: " + (int)team2.Average(p => p.Ranks[GameMode.QuickMatch].Mmr);
			mmr1_container.Style = this.Resources[MyTeam == 0 ? "BlueControl" : "RedControl"] as Style;
			mmr2_container.Style = this.Resources[MyTeam == 1 ? "BlueControl" : "RedControl"] as Style;

			if (autoClose)
				ThreadPool.QueueUserWorkItem(a => {
					Thread.Sleep(10000);
					Dispatcher.BeginInvoke(new Action(() => { Close(); }));
				});
		}
	}

	public class MockViewModel
	{
		public List<PlayerProfile> Players { get; set; }
		public Region Region { get; set; }

		public string MyAccount { get; set; } = "Player 4#123";
		public int MyTeam { get; set; }
		public PlayerProfile Me { get; set; }

		public MockViewModel()
		{
			Players = new List<PlayerProfile> {
				new PlayerProfile("Player 1#123", Region.EU),
				new PlayerProfile("Player 2#123", Region.EU),
				new PlayerProfile("Player 3#123", Region.EU),
				new PlayerProfile("Player 4#123", Region.EU),
				new PlayerProfile("Player 5#123", Region.EU),
			};
			foreach (var p in Players) {
				p.Ranks[GameMode.QuickMatch] = new PlayerProfile.MmrValue(GameMode.QuickMatch, 2200, null, null);
			}
			Me = Players.Where(p => p.BattleTag == MyAccount).FirstOrDefault();
			MyTeam = 0;
		}
	}
}
