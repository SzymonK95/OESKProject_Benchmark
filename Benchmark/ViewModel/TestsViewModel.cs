using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Benchmark.Helpers;
using Benchmark.Model;
using System.Configuration;
using System.Management;
using GalaSoft.MvvmLight.Threading;

namespace Benchmark.ViewModel
{
    public class TestsViewModel : ViewModelBase
    {
        public TestsViewModel()
        {
            SliderRedValue = 1;
            SliderGreenValue = 2;
            SliderBlueValue = 3;
            ProgressBarMaxValue = 10;
            ResultFromDataBase = new ObservableCollection<KeyValuePair<string, double>>();
            DispatcherHelper.Initialize();
            GetProcesorName();
            SetButtonEnabled();
            SetProgressBarValues(0, ProgressBarMaxValue);

            new Thread(Job_GetNoiseFromFile).Start();
        }

        #region Binds

        public string Test1_ButtonContent => "Test 1" + Environment.NewLine + "1M";
        public string Test2_ButtonContent => "Test 2" + Environment.NewLine + "2M";
        public string Test3_ButtonContent => "Test 3" + Environment.NewLine + "3M";
        public string Test4_ButtonContent => "Test 4" + Environment.NewLine + "5M";
        public string Test5_ButtonContent => "Test 5" + Environment.NewLine + "8M";
        public string Test6_ButtonContent => "Test 6" + Environment.NewLine + "10M";

        public String ProcessorName;

        private int sliderRedValue;
        public int SliderRedValue
        {
            get => sliderRedValue;
            set
            {
                if (value != sliderRedValue)
                {
                    sliderRedValue = value;
                    NotifyPropertyChanged(nameof(SliderRedValue));
                }
            }
        }

        private int sliderGreenValue;
        public int SliderGreenValue
        {
            get => sliderGreenValue;
            set
            {
                if (value != sliderGreenValue)
                {
                    sliderGreenValue = value;
                    NotifyPropertyChanged(nameof(SliderGreenValue));
                }
            }
        }

        private int sliderBlueValue;
        public int SliderBlueValue
        {
            get => sliderBlueValue;
            set
            {
                if (value != sliderBlueValue)
                {
                    sliderBlueValue = value;
                    NotifyPropertyChanged(nameof(SliderBlueValue));
                }
            }
        }

        private bool tests_AreEnabled;
        public bool Tests_AreEnabled
        {
            get => tests_AreEnabled;
            set
            {
                if (value != tests_AreEnabled)
                {
                    tests_AreEnabled = value;
                    NotifyPropertyChanged(nameof(Tests_AreEnabled));
                }
            }
        }

        private Int64 progressBarMaxValue;
        public Int64 ProgressBarMaxValue
        {
            get => progressBarMaxValue;
            set
            {
                progressBarMaxValue = value;
                NotifyPropertyChanged(nameof(ProgressBarMaxValue));
            }
        }

        private Int64 progressBarValue;
        public Int64 ProgressBarValue
        {
            get => progressBarValue;
            set
            {
                progressBarValue = value;
                NotifyPropertyChanged(nameof(ProgressBarValue));

            }
        }

        private ObservableCollection<KeyValuePair<string, double>> _resultFromDataBase;
        public ObservableCollection<KeyValuePair<string, double>> ResultFromDataBase
        {
            get
            {
                if (_resultFromDataBase == null)
                {
                    _resultFromDataBase = new ObservableCollection<KeyValuePair<string, double>>();
                }
                return _resultFromDataBase;
            }
            set
            {
                _resultFromDataBase = value;
                NotifyPropertyChanged(nameof(ResultFromDataBase));
            }
        }

        private ObservableCollection<KeyValuePair<string, double>> _resultFromUser;
        public ObservableCollection<KeyValuePair<string, double>> ResultFromUser
        {
            get
            {
                if (_resultFromUser == null)
                {
                    _resultFromUser = new ObservableCollection<KeyValuePair<string, double>>();
                }
                return _resultFromUser;
            }
            set
            {
                _resultFromUser = value;
                NotifyPropertyChanged(nameof(ResultFromUser));
            }
        }

        #endregion

        #region Click

        public void Test1Button_ClickMethod()
        {
            TestStarter((Int64)TestEnum.Test1M);
        }

        public void Test2Button_ClickMethod()
        {
            TestStarter((Int64)TestEnum.Test2M);
        }

        public void Test3Button_ClickMethod()
        {
            TestStarter((Int64)TestEnum.Test3M);
        }

        public void Test4Button_ClickMethod()
        {
            TestStarter((Int64)TestEnum.Test5M);
        }

        public void Test5Button_ClickMethod()
        {
            TestStarter((Int64)TestEnum.Test8M);
        }

        public void Test6Button_ClickMethod()
        {
            TestStarter((Int64)TestEnum.Test10M);
        }

        #endregion Click

        #region Metods

        public void TestStarter(Int64 testSize)
        {
            try
            {
                if (ParametersOK())
                {
                    int rSize = SliderRedValue;
                    int gSize = SliderGreenValue;
                    int bSize = SliderBlueValue;

                    SetProgressBarValues(0, ProgressBarMaxValue);
                    SetButtonEnabled();

                    new Thread(() => Job_MakeOrTakaNoiseAndMakaTest(testSize, rSize, gSize, bSize)).Start();

                    new Thread(Job_ProgressBarValueUpdate).Start();
                }
                else
                {
                    MessageBox.Show("Problem with RGB parameters, becouse all equal zero", "RGB parameters");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void SetButtonEnabled()
        {
            Tests_AreEnabled = !Tests_AreEnabled;
        }

        private void SetProgressBarValues(Int64 value = 0, Int64 maxValue = 100)
        {
            ProgressBarMaxValue = maxValue;
            ProgressBarValue = value;
        }

        private double? GetAvgResultByTestSize(Int64 testSize)
        {
            try
            {
                if (lastTestSize != testSize)
                    using (var db = new BenchmarkDBEntities())
                        return db.TestViews.Where(x => x.TestSize == testSize && x.ProcessorName == ProcessorName).Average(x => x.AvarageTime);
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        #endregion

        #region Job

        private String noise = String.Empty;
        private Int64 lastTestSize;
        private void Job_MakeOrTakaNoiseAndMakaTest(Int64 testSize, int rSize, int gSize, int bSize)
        {
            new Thread(() => Job_SetResultlistItemsFromDatabase(testSize)).Start();

            if (noise.Length < testSize)
            {
                StringBuilder sb = new StringBuilder();

                while (sb.Length < testSize)
                {
                    sb.Append(noise);
                }

                noise = sb.ToString();
            }

            List<double> results = new List<double>();

            for (ProgressBarValue = 0; ProgressBarValue < ProgressBarMaxValue; ProgressBarValue++)
                results.Add(EncryptionManager.Encrypt(noise.Substring(0, (int)testSize), rSize, gSize, bSize));

            DispatcherHelper.CheckBeginInvokeOnUI
                (() =>
                {
                    ResultFromUser.Clear();
                    ResultFromUser.Add(new KeyValuePair<string, double>
                    (
                        ProcessorName,
                        (results.Sum() / results.Count)
                    ));
                });

            lastTestSize = testSize;

            new Thread(() => Job_SaveTestResult(results, testSize, rSize, gSize, bSize)).Start();
        }

        private void Job_SetResultlistItemsFromDatabase(Int64 testSize)
        {
            var result = GetAvgResultByTestSize(testSize);

            if (result != null)
                DispatcherHelper.CheckBeginInvokeOnUI
                (() =>
                {
                    ResultFromDataBase.Clear();
                    ResultFromDataBase.Add(new KeyValuePair<string, double>
                    (
                        ProcessorName,
                        result.Value
                    ));
                });
        }

        private void Job_ProgressBarValueUpdate()
        {
            while (ProgressBarValue < ProgressBarMaxValue)
            {
                Thread.Sleep(100);
            }

            SetButtonEnabled();
            SetProgressBarValues(0, ProgressBarMaxValue);
        }

        private void Job_SaveTestResult(List<double> resultsList, Int64 testSize, int rSize, int gSize, int bSize)
        {
            string filePath = @"Test_" + testSize + "_RGB_" + rSize + gSize + bSize + "_" + DateTime.Now.Ticks + ".csv";

            StringBuilder sb = new StringBuilder();
            foreach (var result in resultsList)
            {
                sb.AppendLine(result.ToString());
            }

            sb.AppendLine("Średnia");
            var avg = (resultsList.Sum() / resultsList.Count);

            new Thread(() => Job_SaveResultInDataBase(avg, testSize)).Start();

            sb.AppendLine(avg.ToString());

            File.WriteAllText(filePath, sb.ToString());
        }

        private void Job_GetNoiseFromFile()
        {
            try
            {
                string path = ConfigurationManager.AppSettings["Path"];
                noise = File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                noise = NoiseManager.GenerateNoise(10000);
                MessageBox.Show(ex.Message, "Problem with input file");
            }
        }

        private void Job_SaveResultInDataBase(double avg, long testSize)
        {
            using (var db = new BenchmarkDBEntities())
            {
                int processorId;
                try
                {
                    processorId = db.processors.Where(x => x.Name.Contains(ProcessorName)).First().Id;
                }
                catch (Exception ex)
                {
                    processorId = 0;
                }

                if (processorId == 0)
                {
                    processorId = db.processors.Count() + 1;

                    db.processors.Add(new processor
                    {
                        Id = processorId,
                        Name = ProcessorName
                    });
                }

                db.tests.Add(new test
                {
                    Id = db.tests.Count() + 1,
                    AvarageTime = avg,
                    TestSize = (int)testSize,
                    DateTimeOfTest = DateTime.Now,
                    ProcesorId = processorId
                });

                db.SaveChanges();
            }
        }

        private void GetProcesorName()
        {
            string name = "";
            ManagementObjectSearcher mos =
                new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                name = mo["Name"].ToString();
            }

            ProcessorName = name;
            NotifyPropertyChanged(nameof(ProcessorName));
        }

        #endregion

        #region Validation

        private bool ParametersOK()
        {
            return !(SliderRedValue == 0 && SliderGreenValue == 0 && SliderBlueValue == 0);
        }

        #endregion

        #region ICommand 

        public ICommand Test1Button_ClickCommand => new Utils.RelayCommand(Test1Button_ClickMethod, AlwaysTrue);
        public ICommand Test2Button_ClickCommand => new Utils.RelayCommand(Test2Button_ClickMethod, AlwaysTrue);
        public ICommand Test3Button_ClickCommand => new Utils.RelayCommand(Test3Button_ClickMethod, AlwaysTrue);
        public ICommand Test4Button_ClickCommand => new Utils.RelayCommand(Test4Button_ClickMethod, AlwaysTrue);
        public ICommand Test5Button_ClickCommand => new Utils.RelayCommand(Test5Button_ClickMethod, AlwaysTrue);
        public ICommand Test6Button_ClickCommand => new Utils.RelayCommand(Test6Button_ClickMethod, AlwaysTrue);

        #endregion
    }
}
