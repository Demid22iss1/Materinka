using System;
using System.Drawing;
using System.Linq;
using System.Management; // Для работы с системными ресурсами
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form //объявление класса Form1, который является подклассом класса Form. Это означает, что класс Form1 наследует функциональность класса Form
    {
        public Form1() //конструктор класса Form1. Он вызывается при создании нового экземпляра класса Form1.
        {
            InitializeComponent(); //этот код гарантирует, что все компоненты формы будут правильно инициализированы при создании нового экземпляра класса Form1
        }
///////ПРОЦЕССОР////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void pictureBox1_Click(object sender, EventArgs e) //объявление метода pictureBox1_Click(), который будет вызываться при щелчке на pictureBox1
        {
            string processorInfo = GetProcessorInfo(); // Получаем информацию о процессоре
            label1.Text = "" + processorInfo; // Устанавливаем текст метки для отображения информации о процессоре
        }

        private static string GetProcessorInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor"); //создание объекта searcher типа ManagementObjectSearcher для поиска информации о процессоре
            ManagementObject obj = searcher.Get().OfType<ManagementObject>().FirstOrDefault(); //получение первого объекта ManagementObject из результатов поиска

            if (obj != null) //проверка, что объект был найден
            {
                string name = obj["Name"].ToString(); //получение имени процессора из свойства "Name" объекта obj и преобразование его в строку
                string maxClockSpeed = (Convert.ToDouble(obj["MaxClockSpeed"]) / 1000).ToString("0.0") + " Ггц"; //получение максимальной тактовой частоты процессора из свойства "MaxClockSpeed" объекта obj, деление на 1000 для получения значения в гигагерцах
                int numberOfCores = Convert.ToInt32(obj["NumberOfCores"]); // получение количества ядер процессора из свойства "NumberOfCores" объекта obj и преобразование его в целое число
                int numberOfLogicalProcessors = Convert.ToInt32(obj["NumberOfLogicalProcessors"]); //получение количества логических процессоров процессора

                string info = $"{name}\n" +
                              $"Число ядер: {numberOfCores}, потоков {numberOfLogicalProcessors};\n" +
                              $"Частота: {maxClockSpeed}\n";

                return info; //возврат значения переменной info (т.е. информации о процессоре) из метода GetProcessorInfo()
            }
            else
            {
                return string.Empty; // Возвращаем пустую строку в случае отсутствия объекта
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // Пустой обработчик события загрузки формы
        }

        private void Form1_Load1(object sender, EventArgs e)
        {
            string processorInfo = GetProcessorInfo(); // Получаем информацию о процессоре


            // Выводим отладочные сообщения для проверки значений
            Console.WriteLine("Processor Info: " + processorInfo);


            // Устанавливаем изображение в PictureBox в зависимости от производителя процессора
            if (processorInfo.Contains("Intel"))
            {
                pictureBox1.Image = Properties.Resources.logo_intel2;
            }
            else if (processorInfo.Contains("AMD"))
            {
                pictureBox1.Image = Properties.Resources.logo_AMD1;
            }


            string query = "SELECT * FROM Win32_Processor";




            label1.Text = processorInfo; // Устанавливаем текст метки для отображения информации о процессоре

        }
        


////////ОПЕРАТИВКА/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            string ramInfo = GetRAMInfo(); // Получаем информацию о ОЗУ
            label2.Text = "" + ramInfo; // Устанавливаем текст метки для отображения информации о ОЗУ
        }
        
        private static string GetRAMInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
            string memoryType = "";
            long totalCapacity = 0;
            string frequency = "";
            bool hasFreeSlots = false;

            foreach (ManagementObject obj in searcher.Get())
            {
                int memoryTypeValue = Convert.ToInt32(obj["MemoryType"]); //преобразование значения MemoryType из объекта в тип int и присвоение этого значения переменной memoryTypeValue

                // Определяем тип оперативной памяти по числовому значению MemoryType
                switch (memoryTypeValue) //блок кода, который сравнивает значение переменной memoryTypeValue с различными числовыми константами, чтобы определить тип оперативной памяти
                {
                    case 20: 
                        memoryType = "DDR"; //для значения 20 переменная memoryType устанавливается в "DDR".
                        break;
                    case 21:
                        memoryType = "DDR2"; 
                        break; 
                    case 24:
                        memoryType = "DDR3";
                        break;
                    case 26:
                        memoryType = "DDR4";
                        break;
                    case 30:
                        memoryType = "DDR5";
                        break;
                    default:
                        memoryType = "Неизвестно";
                        break;
                }

                long capacityBytes = Convert.ToInt64(obj["Capacity"]); //преобразование значения Capacity из объекта в тип long и присвоение этого значения переменной capacityBytes
                totalCapacity += capacityBytes; //операция сложения, при которой значение capacityBytes добавляется к переменной totalCapacity
                frequency = obj["ConfiguredClockSpeed"].ToString() + " МГц"; //присваивание переменной frequency значения ConfiguredClockSpeed из объекта в виде строки

                // Дополнительная проверка на свободные слоты для оперативной памяти
                if (Convert.ToInt32(obj["ConfiguredClockSpeed"]) == 0)
                {
                    hasFreeSlots = true;
                }
            }

            // Преобразуем байты в гигабайты и форматируем строку
            string ramInfo = $"Тип оперативной памяти: {memoryType}\n" +
                             $"Объем оперативной памяти: {(totalCapacity / (1024 * 1024 * 1024))} GB\n" +
                             $"Частота оперативной памяти: {frequency}\n" +
                             $"Свободные слоты для оперативной памяти: {(hasFreeSlots ? "Да" : "Нет")}";

            return ramInfo;
            
        }
////////ВИДЕОКАРТА/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            string gpuInfo = GetVideoCardInfo(); // Получаем информацию о видеокарте
            label3.Text = "" + gpuInfo; // Устанавливаем текст метки для отображения информации о видеокарте
            
            label3.Text = $"Видеокарта: {gpuInfo}";

            // Проверяем имя производителя видеокарты
            if (gpuInfo.Contains("ASUS"))
            {
                pictureBox3.Image = Properties.Resources.vid_asus2; 
            }
            else if (gpuInfo.Contains("GIGABYTE"))
            {
                pictureBox3.Image = Properties.Resources.vid_gigabyte2; 
            }
            else if (gpuInfo.Contains("MSI"))
            {
                pictureBox3.Image = Properties.Resources.vid_msi2; 
            }
            else if (gpuInfo.Contains("AMD Radeon"))
            {
                pictureBox3.Image = Properties.Resources.logo_radeon; 
            }
            else if (gpuInfo.Contains("SAPPHIRE"))
            {
                pictureBox3.Image = Properties.Resources.vid_sapphire2; 
            }
            else if (gpuInfo.Contains("NVIDIA"))
            {
                pictureBox3.Image = Properties.Resources.logo_NVIDIA; 
            }
            else
            {
                pictureBox3.Image = null; 
            }
        }

        private string GetVideoCardInfo()
{
            string videoCardInfo = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"); //используется для выполнения запроса WMI для получения информации о видеоконтроллерах
            foreach (ManagementObject obj in searcher.Get()) //обрабатываются все объекты ManagementObject, полученные из запроса
            {
                string videoCardName = obj["Name"].ToString(); //название видеокарты
                ulong videoCardAdapterRAM = Convert.ToUInt64(obj["AdapterRAM"]); //объем видеопамяти (в байтах)
                string videoCardGPU = obj["Description"].ToString(); //описание графического процессора
                uint videoCardStreamProcessors = Convert.ToUInt32(obj["MaxNumberControlled"]); //количество потоковых процессоров

                double adapterRAMinGB = Math.Round(videoCardAdapterRAM / (1024.0 * 1024.0 * 1024.0), 2); //преобразование объема видеопамяти из байтов в гигабайты

                videoCardInfo = $"{videoCardName}\nГрафический процессор: {videoCardGPU}\n Объем памяти: {adapterRAMinGB} ГБ";
                break; // Прерываем цикл после первой найденной видеокарты
            }
             
            return videoCardInfo; //возвращается строка videoCardInfo, содержащая информацию о найденной видеокарте
        }
////////ЖЁСТКИЙ ДИСК/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            string hddInfo = GetHardDriveInfo(); // Получаем информацию о жёстком диске
            label4.Text = "" + hddInfo; // Устанавливаем текст метки для отображения информации о жёстком диске
            if (hddInfo.Contains("intel"))
            {
                pictureBox4.Image = Properties.Resources.ssd_intel;
            }
            else if (hddInfo.Contains("KINGSTON"))
            {
                pictureBox4.Image = Properties.Resources.ssd_kingston;
            }
            else if (hddInfo.Contains("PATRIOT"))
            {
                pictureBox4.Image = Properties.Resources.ssd_patriot;
            }
            else if (hddInfo.Contains("SanDisk"))
            {
                pictureBox4.Image = Properties.Resources.ssd_sandisk;
            }
            else if (hddInfo.Contains("Western Digital"))
            {
                pictureBox4.Image = Properties.Resources.ssd_WD;
            }
            else if (hddInfo.Contains("Samsung"))
            {
                pictureBox4.Image = Properties.Resources.ssd_samsung;
            }
            else if (hddInfo.Contains("Стандартные дисковые накопители"))
            {
                pictureBox4.Image = Properties.Resources.ssd;
            }
            else
            {
                pictureBox4.Image = null; // Если имя производителя не найдено, оставляем pictureBox пустым
            }
            

        }

        private string GetHardDriveInfo()
        {
            StringBuilder hardDriveInfo = new StringBuilder();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            foreach (ManagementObject obj in searcher.Get())
            {
                string manufacturer = obj["Manufacturer"]?.ToString();
                string size = Convert.ToInt64(obj["Size"]) / (1024 * 1024 * 1024) + " GB";
                string interfaceType = obj["InterfaceType"]?.ToString();

                hardDriveInfo.AppendLine($"Производитель: {manufacturer}");
                hardDriveInfo.AppendLine($"Тип накопителя: {interfaceType}");
                hardDriveInfo.AppendLine($"Объем накопителя: {size}\n");
            }
            return hardDriveInfo.ToString();
        }

    }
}