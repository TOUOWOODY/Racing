using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
//using Firebase;
//using Firebase.Unity.Editor;
//using Firebase.Database;
using UnityEngine;

public static class TableHandler
{
    public enum SteamMode
    {
        FireBase, 
        AppData,
        Resource,
        Sample
    }

    public struct ColumnInfo
    {
        public int Index;
        public string Type;
        public string name;
    }

    public class Row
    {
        private Table table = null;
        private List<string> data = new List<string>();

        public Row(Table table, List<string> data)
        {
            this.table = table;
            this.data.AddRange(data);
        }

        public T Get<T>(string name)
        {
            int targetIndex = table.GetIndex(name);

            if(targetIndex == -1)
            {
                T returnValue = (T)Convert.ChangeType(0, typeof(T));
                return returnValue;
            }
            else
            {
                if (data.Count <= targetIndex)
                {
                    for (int i = data.Count; i <= targetIndex; i++)
                    {
                        data.Add("0");
                        //Debug.Log(data.Count+" "+targetIndex);
                    }
                    table.Save();
                    //data[targetIndex] = "0";
                }
                
                //Debug.LogFormat("count : {0}, TIndex : {1}", data.Count, targetIndex);
                T returnValue = (T)Convert.ChangeType(data[targetIndex], typeof(T));
                return returnValue; //여기서 터진다면 테이블에서 마지막에 0,0,0 이런거 있나 찾아볼것
            }
        }
    
        public Dictionary<string, T> QueryByContainedString<T>(string value)
        {
            Dictionary<string, T> returnTable = new Dictionary<string, T>();

            List<string> matched = new List<string>();
            foreach (var key in table.ColumnHeader.Keys)
            {
                if(key.Contains(value))
                {
                    matched.Add(key);
                }
            }

            foreach (var key in matched)
            {
                returnTable.Add(key, Get<T>(key));
            }

            return returnTable;
        }

        public T GetAt<T>(int index)
        {
            return (T)Convert.ChangeType(data[index], typeof(T));
        }

        public List<string> GetAllData()
        {
            return data;
        }

        public bool Replace(List<string> newValue)
        {
            data.Clear();
            data.AddRange(newValue);
            return true;
        }

        public int Remove() {
            data.Clear();

            return 0;
        }

        //public void Swap(Row index) {
        //    var temp = data;
        //    data[index] = data[index + 1];
        //    data[index + 1] = temp;            
        //}

        public void ReplaceColumn(string columnName, string newValue)
        {
            int targetIndex = table.GetIndex(columnName);

            if(targetIndex == -1)
            {
                Debug.LogWarning("TargetColumn Does not Exist" + columnName);
            }
            else
            {
                data[table.GetIndex(columnName)] = newValue;
            }
        }

        public void Save()
        {
            table.Save();
        }
    }

    public class Table
    {
        private Dictionary<string, ColumnInfo> columnHeader = new Dictionary<string, ColumnInfo>();
        private List<Row> rows = new List<Row>();
        private string path = null;
        private string name = "";
        //private List<List<string>> data = new List<List<string>>();
            
        public List<Row> Rows { get { return rows; } }
        public Dictionary<string, ColumnInfo> ColumnHeader {  get { return columnHeader; } }

        public Table(string TableName, SteamMode LoadMode)
        {
            //if (Application.platform == RuntimePlatform.Android)
            //{
            //}
            //else if (Application.platform == RuntimePlatform.IPhonePlayer)
            //{
            //}
            //else
            //{
            //}

            name = TableName;

            //if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            //{
            //    ResourcePath = "Table/";
            //    SamplePath = "Table/Sample/";
            //}
            //else
            //{
            //    ResourcePath = "Assets/StarShip/Resources/Table/";
            //    SamplePath = "Assets/StarShip/Resources/Table/Sample/";
            //}

            switch (LoadMode)
            {
                case SteamMode.FireBase:
                    break;
                case SteamMode.AppData:
                    path = AppDataPath;
                    break;
                case SteamMode.Resource:
                    path = ResourcePath;
                    break;
                case SteamMode.Sample:
                    path = SamplePath;
                    break;
            }

            path += (TableName); //+ ".csv" );



            if (!System.IO.File.Exists(path + ".csv"))
            {
                // Create
                // 테이블 참조하는 스타일이 다르다. 지금 가정은 무조건 ㅇ=
                switch (LoadMode)
                {
                    case SteamMode.FireBase:
                        // Not implemented
                        break;
                    case SteamMode.AppData:
                        // 엡데이타에 없으면 샘플을 불러오자.
                        path = SamplePath + TableName;// + ".csv";
                        LoadMode = SteamMode.Sample;
                        break;
                    case SteamMode.Resource:
                        // Error! break;
                        //UnityEngine.Debug.LogWarning("The table not exist in Resource! " + TableName);
                        break;
                }
            }

            CsvFile.CsvFileReader reader;// = new CsvFile.CsvFileReader(stream)

            //Stream stream = new MemoryStream();
            //FileStream file = new FileStream();
            if (LoadMode == SteamMode.AppData)
            {
                FileStream file = new FileStream(path + ".csv", FileMode.Open);
                //file.CopyTo(stream);
                reader = new CsvFile.CsvFileReader(file);
            }
            else
            {
                TextAsset table = Resources.Load<TextAsset>(path);
                Stream stream = new MemoryStream(table.bytes);
                reader = new CsvFile.CsvFileReader(stream);

            }

            //CsvFile.CsvFileReader asdasd = new CsvFile.CsvFileReader(file);

            using (reader)
            {
                int index = 0;
                List<string> column = new List<string>();
                while (reader.ReadRow(column))
                {
                    // Column Header.
                    if (index == 0)
                    {
                        for (int i = 0; i < column.Count; i++)
                        {
                            if (column[i].Contains("s_"))
                            {
                                columnHeader.Add(column[i].Replace("s_", ""),new ColumnInfo() { Index = i, Type = "string", name = column[i] });
                            }
                            else if (column[i].Contains("i_"))
                            {
                                columnHeader.Add(column[i].Replace("i_", ""), new ColumnInfo() { Index = i, Type = "int", name = column[i] });
                            }
                            else if (column[i].Contains("f_"))
                            {
                                columnHeader.Add(column[i].Replace("f_", ""), new ColumnInfo() { Index = i, Type = "float", name = column[i] });
                            }
                            else if( column[i].Contains("b_"))
                            {
                                columnHeader.Add(column[i].Replace("b_", ""), new ColumnInfo() { Index = i, Type = "bool", name = column[i] });
                            }
                            else
                            {
                                columnHeader.Add(column[i], new ColumnInfo() { Index = i, Type = "string", name = column[i] });
                            }
                        }
                    }
                    // Datas
                    else
                    {
                        rows.Add(new Row(this, column));
                    }
                    index++;
                }
            }
        }
        
        public void Add(List<string> item)
        {
            int newIndex = Rows.Count + 1;
            item[0] = Convert.ToString(newIndex);
            rows.Add(new Row(this, item));
        }

        public void AddRow(List<string> item)
        {
            rows.Add(new Row(this, item));
        }

        public bool Save(SteamMode SaveMode = SteamMode.AppData)
        {
            switch (SaveMode)
            {
                case SteamMode.AppData:
                    path = (AppDataPath+ name + ".csv" );
                    break;
                case SteamMode.Resource:
                case SteamMode.FireBase:
                case SteamMode.Sample:
                    return false;
                    //break; // I know but consistency
            }

            return Save(path);
        }

        public bool FireBaseTableSaveAll()
        {
           // DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            //reference.Child("users").Child(userId).
            return true;
        }

        public bool Save(string path)
        {
            if(!System.IO.File.Exists(path))
            {
                if (!System.IO.Directory.Exists(AppDataPath))
                {
                    System.IO.Directory.CreateDirectory(AppDataPath);
                }

                using (System.IO.FileStream fs = System.IO.File.Create(path))
                {
                }
            }
            
            using (var writer = new CsvFile.CsvFileWriter(path))
            {
                List<string> columns = new List<string>(columnHeader.Count);

                var headerEnumerator = columnHeader.Values.GetEnumerator();
                while(headerEnumerator.MoveNext())
                {
                    var current = headerEnumerator.Current;
                    columns.Add(current.name);
                }

                writer.WriteRow(new List<string>(columns));
                columns.Clear();

                foreach (Row row in rows)
                {
                    columns.AddRange(row.GetAllData());
                    writer.WriteRow(new List<string>(columns));
                    columns.Clear();
                }
            }

            return true;
        }

        public Row GetAt(int index)
        {
            return rows[index];
        }

        //public Row GetRowByBPIndex(int bPindex) {
        //    Row target = FindRow("blueprintIndex", bPindex);

        //    if ( target == null )
        //        return null;

        //    return target;
        //}

        public Row FindRow<T>(string columnName, T condition)
        {
            
            return rows.Find(x => Compare(x.Get<T>(columnName), condition));
        }

        public bool Replace<T>(string columnName, T condition, List<string> newValue)
        {
            Row target = FindRow(columnName, condition);
            return target.Replace(newValue);
        }

        // 현 테이블에 있는 모든 Data를 제거한다.
        public void Refresh()
        {
            rows.Clear();
        }

        public List<Row> FindRows<T>(string columnName, T condition)
        {
            return rows.FindAll(x => Compare(x.Get<T>(columnName), condition));
        }

        public ColumnInfo GetColumnInfo(string name)
        {
            return columnHeader[name];
        }

        public void Remove(int index) {
            int max = rows.Count;

            if ( max < index ) {
                Debug.Log("Delete Error");
                return;
            }

            rows.RemoveAt(index - 1);
            for ( int i = index - 1; i < max - 1; i++ ) {
                rows[i].ReplaceColumn("index", ( i + 1 ).ToString());
            }
        }

        public int GetIndex(string name)
        {
            //Debug.LogFormat("({0}) GetIndex From ({1})", name, this.name);
            if(columnHeader.ContainsKey(name))
            {
                return columnHeader[name].Index;
            }
            else
            {
                return -1;
            }
        }

        public bool Compare<T>(T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }
    }

    // 필요한 기능 1 : 테이블을 로드하고, 이를 메모리에 케시 해두고 손쉽게 접근 가능해야 함.
    // 필요한 기능 2 : 로드한 테이블의 개별 데이터에 접근이 쉬워야 함.

    // 필요한 기능 3 : 로드한 테이블의 일부 내용을 수정하고, 이를 메모리/파일에 저장해야 함.
    // 필요한 기능 4 : 수정된 테이블에 다시 접근하여 데이터를 가져올 때, 수정된 부분이 반영되야 함.
    private static Dictionary<string, Table> tableMap = new Dictionary<string, Table>();

    public static string AppDataPath { get { return string.Format("{0}/Table/", UnityEngine.Application.persistentDataPath); } }
    public static string SamplePath = "Table/Sample/";
    public static string ResourcePath = "Table/";

    private static Table GenerateTable(string tableName, SteamMode LoadSteam)
    {
        return new Table(tableName, LoadSteam);
    }

    public static Table Get(string tableName, SteamMode LoadSteam)
    {
        if ( !tableMap.ContainsKey(tableName) ) {
            Table newTable = GenerateTable(tableName, LoadSteam);
            tableMap.Add(tableName, newTable);
        } else {
            if ( LoadSteam == SteamMode.AppData ) {
                Table newTable = GenerateTable(tableName, LoadSteam);
                tableMap[tableName] = newTable;
            }
        }
        return tableMap[tableName];
    }

    public static void RefreshTableHeader(string tableName)
    {
        Table newTable = GenerateTable(tableName, SteamMode.Sample);
        //Table tempTable;

        if (newTable.Rows.Count == 0)
        {
            return;
        }
        newTable.Rows.RemoveRange(0, newTable.Rows.Count);
        newTable.Rows.AddRange(tableMap[tableName].Rows);

        tableMap[tableName] = newTable;
        TableHandler.Save(tableName);
    }

    public static bool Save(string tableName, SteamMode saveMode = SteamMode.AppData)
    {
        bool ReturnValue = false;
        if (!tableMap.ContainsKey(tableName))
        {
            //Warning!
            return ReturnValue;
        }

        switch (saveMode)
        {
            case SteamMode.AppData:
                ReturnValue = tableMap[tableName].Save(string.Format("{0}{1}.csv", AppDataPath, tableName));
                break;
            case SteamMode.FireBase:
            case SteamMode.Resource:
            case SteamMode.Sample:
                Debug.Log("Not Supported : (" + tableName + ") " + saveMode);
                break;
        }

        return ReturnValue;
    }
}
