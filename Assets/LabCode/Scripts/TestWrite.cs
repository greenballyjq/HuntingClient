using UnityEngine;
using System.IO;
using System.Text;

namespace LabCode.Scripts
{
    /// <summary>
    /// 测试写入Excel文件(.xls格式)
    /// </summary>
    public class TestWrite : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private string fileName = "TestOutput";
        [SerializeField] private int testRowCount = 10;
        [SerializeField] private int testColumnCount = 5;
        
        [Header("调试信息")]
        [SerializeField] private string lastSavedPath;

        private void Start()
        {
            // 自动执行测试
            CreateTestExcelFile();
        }

        [ContextMenu("创建测试Excel文件")]
        public void CreateTestExcelFile()
        {
            CreateTestExcelFile(testRowCount, testColumnCount, fileName);
        }

        /// <summary>
        /// 创建测试Excel文件
        /// </summary>
        public string CreateTestExcelFile(int rows, int columns, string customFileName)
        {
            // 获取保存路径
            string savePath = GetSavePath(customFileName);
            
            // 创建Excel数据
            byte[] excelData = CreateXlsData(rows, columns);
            
            // 写入文件
            try
            {
                File.WriteAllBytes(savePath, excelData);
                lastSavedPath = savePath;
                Debug.Log($"Excel文件已成功创建: {savePath}");
                return savePath;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建Excel文件失败: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 获取保存路径
        /// </summary>
        private string GetSavePath(string customFileName)
        {
            string directory = Path.Combine(Application.persistentDataPath, "ExcelExports");
            
            // 确保目录存在
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            return Path.Combine(directory, $"{customFileName}.xls");
        }

        /// <summary>
        /// 创建XLS格式的Excel数据
        /// </summary>
        private byte[] CreateXlsData(int rows, int columns)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            
            // XLS文件头
            WriteXlsHeader(writer);
            
            // 写入工作表信息
            WriteWorkbook(writer, rows, columns);
            
            return stream.ToArray();
        }

        /// <summary>
        /// 写入XLS文件头
        /// </summary>
        private void WriteXlsHeader(BinaryWriter writer)
        {
            // BOF记录 (Beginning of File)
            writer.Write((ushort)0x0009); // BOF记录类型
            writer.Write((ushort)0x0004); // 记录长度
            writer.Write((ushort)0x0500); // Excel版本标识
            writer.Write((ushort)0x0006); // 文件类型标识
            
            // 写入一些必要的记录
            WriteRecord(writer, 0x000E, 0x0000, new byte[0]); // EOF
        }

        /// <summary>
        /// 写入工作簿和工作表
        /// </summary>
        private void WriteWorkbook(BinaryWriter writer, int rows, int columns)
        {
            // 创建一个简单的XLS结构
            // 注意：这是一个最小化的XLS实现，仅用于基本测试
            
            // 写入工作表
            WriteWorksheet(writer, rows, columns);
            
            // 结束记录
            WriteRecord(writer, 0x000A, 0x0000, new byte[0]); // EOF
        }

        /// <summary>
        /// 写入工作表数据
        /// </summary>
        private void WriteWorksheet(BinaryWriter writer, int rows, int columns)
        {
            // 创建工作表头
            WriteRecord(writer, 0x0809, 0x0004, new byte[] { 0x00, 0x10, 0x00, 0x00 }); // BOF (Worksheet)
            
            // 写入列信息
            for (int col = 0; col < columns; col++)
            {
                WriteColumnInfo(writer, col);
            }
            
            // 写入行数据
            for (int row = 0; row < rows; row++)
            {
                WriteRowData(writer, row, columns);
            }
            
            // 结束工作表
            WriteRecord(writer, 0x000A, 0x0000, new byte[0]); // EOF
        }

        /// <summary>
        /// 写入列信息
        /// </summary>
        private void WriteColumnInfo(BinaryWriter writer, int columnIndex)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter colWriter = new BinaryWriter(ms);
            
            colWriter.Write((ushort)columnIndex); // 第一列
            colWriter.Write((ushort)columnIndex); // 最后一列
            colWriter.Write((ushort)0x0000);     // 列宽
            colWriter.Write((ushort)0x0000);     // XF索引
            colWriter.Write((ushort)0x0000);     // 选项
            colWriter.Write((ushort)0x0000);     // 未使用
            
            WriteRecord(writer, 0x007D, (ushort)ms.Length, ms.ToArray());
        }

        /// <summary>
        /// 写入行数据
        /// </summary>
        private void WriteRowData(BinaryWriter writer, int rowIndex, int columnCount)
        {
            // 写入行记录
            MemoryStream ms = new MemoryStream();
            BinaryWriter rowWriter = new BinaryWriter(ms);
            
            rowWriter.Write((ushort)rowIndex);   // 行索引
            rowWriter.Write((ushort)0x0000);     // 第一列
            rowWriter.Write((ushort)(columnCount - 1)); // 最后一列
            rowWriter.Write((ushort)0x0000);     // 行高度
            rowWriter.Write((ushort)0x0000);     // 未使用
            rowWriter.Write((ushort)0x0000);     // 默认
            
            WriteRecord(writer, 0x0208, (ushort)ms.Length, ms.ToArray());
            
            // 写入单元格数据
            for (int col = 0; col < columnCount; col++)
            {
                WriteCellData(writer, rowIndex, col);
            }
        }

        /// <summary>
        /// 写入单元格数据
        /// </summary>
        private void WriteCellData(BinaryWriter writer, int row, int col)
        {
            // 创建一个简单的测试数据
            string testData = $"数据{row + 1}-{col + 1}";
            
            // 写入字符串标签
            MemoryStream ms = new MemoryStream();
            BinaryWriter cellWriter = new BinaryWriter(ms);
            
            cellWriter.Write((ushort)row);       // 行
            cellWriter.Write((ushort)col);       // 列
            cellWriter.Write((ushort)0x0000);    // XF索引
            byte[] stringBytes = System.Text.Encoding.Unicode.GetBytes(testData);
            cellWriter.Write((byte)stringBytes.Length); // 字符串长度
            cellWriter.Write(stringBytes);       // 字符串内容
            
            WriteRecord(writer, 0x0204, (ushort)ms.Length, ms.ToArray());
        }

        /// <summary>
        /// 写入记录
        /// </summary>
        private void WriteRecord(BinaryWriter writer, ushort recordType, ushort recordLength, byte[] data)
        {
            writer.Write(recordType);
            writer.Write(recordLength);
            if (data.Length > 0)
            {
                writer.Write(data);
            }
        }
        
        [ContextMenu("打开保存目录")]
        public void OpenSaveDirectory()
        {
            string directory = Path.Combine(Application.persistentDataPath, "ExcelExports");
            
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            Application.OpenURL("file://" + directory);
        }
    }
}