using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Cfix.Control;
using Cfix.Control.RunControl;
using Cfix.Control.Native;
using Cfix.Control.Ui.Explorer;

namespace QuickTest
{
	public partial class ExplorerForm : Form
	{
		private AgentSet ooTarget;
		private Agent inTarget;

		public TestExplorer Explorer
		{
			get
			{
				return this.explorer;
			}
		}

		public ExplorerForm()
		{
			InitializeComponent();

			this.explorer.AfterSelected += delegate(
				Object sender,
				ExplorerNodeEventArgs args
				)
			{
				this.currentNodeLabel.Text = args.Item.Name;
			};

			this.ooTarget = new AgentSet();
			this.ooTarget.AddArchitecture( Agent.CreateLocalAgent(
				Architecture.I386,
				false ) );

			this.inTarget = Agent.CreateLocalAgent(
				Architecture.I386,
				true );
		}

		private void refresh_Click( object sender, EventArgs e )
		{
			Explorer.RefreshSession( true );
		}

		private void timer1_Tick( object sender, EventArgs e )
		{
			if ( this.progressLabel.Text.Length > 40 )
			{
				this.progressLabel.Text = "";
			}
			else
			{
				this.progressLabel.Text += ".";
			}
		}

		private void abortBtn_Click( object sender, EventArgs e )
		{
			this.explorer.AbortRefreshSession();
		}

		private delegate void VoidDelegate();

		public void LoadModule( string path )
		{
			IHost host = inTarget.CreateHost();

			using ( ITestItemCollection mod = host.LoadModule(
				null,
				path,
				false ) )
			{
				IRunCompiler comp = new SimpleRunCompiler(
					ooTarget,
					new StandardDispositionPolicy(
							Disposition.Continue, Disposition.Break ),
					SchedulingOptions.None,
					ThreadingOptions.None );
				comp.Add( ( IRunnableTestItem ) mod );
				IRun run = comp.Compile();

				//this.Explorer.SetSession( new Session(), true );
				//this.Explorer.Session.Tests = mod;

				run.StatusChanged += delegate( object sender, EventArgs e )
				{
					this.BeginInvoke( ( VoidDelegate ) delegate
					{
						this.statusTxt.Text = String.Format(
							"{0}/{1}", run.ItemsCompleted, run.ItemCount );
					} );
				};

				this.Results.Run = run;
				run.Start();
			}
		}

		private void button1_Click( object sender, EventArgs e )
		{
			LoadModule( @"D:\dev\wdev\cfixplus\trunk\bin\chk\i386\testmanaged.dll" );
		}

		private void button2_Click( object sender, EventArgs e )
		{
			LoadModule( @"D:\dev\wdev\cfixplus\trunk\bin\chk\i386\testslow.dll" );
		}

		private void button3_Click( object sender, EventArgs e )
		{
			LoadModule( @"D:\dev\wdev\cfixplus\trunk\bin\chk\i386\testctl.dll" );
		}

		private void button4_Click( object sender, EventArgs e )
		{
			this.Results.Run.Terminate();
		}

		private void button5_Click( object sender, EventArgs e )
		{
			this.Results.Run.Stop();
		}
	}
}