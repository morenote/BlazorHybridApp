using LiteDB;

using MoreNote.MSync.Services.FileSystem;
using MoreNote.MSync.Services.FileSystem.IMPL;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreNote.MauiLib.Models
{
    // 创建你的 POCO 类
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
    }

    public class LocalRepository
    {
        /// <summary>
        /// 基路径，可以认为是仓库文件夹的路径 带/
        /// </summary>
        public string? BasePath { get; set; }
        public VirtualFileSystem fileSystemServices=new LocalFileSystem();
        public string RepositoryConfigFile { get; set; }="config";

        public string GetConfigFilePath()
        {
            return BasePath + RepositoryConfigFile;
        }
        public string DataDir { get {string path=Path.Combine(BasePath,"Data") ;return path;} }
        public string HistoryDir { get {string path=Path.Combine(BasePath, "History") ;return path;} }
        public string ConfigDir { get {string path=Path.Combine(BasePath, "Config") ;return path;} }
        public string DataBase { get {string path=Path.Combine(DataDir, "litedb.db") ;return path;} }

        public static LocalRepository Open(string basePath)
        {
            LocalRepository localRepository = new LocalRepository();
            localRepository.BasePath = basePath;
            return localRepository;
        }
        /// <summary>
        /// 初始化仓库
        /// </summary>
        public void Init()
        {
            //首先根据配置文件判断是是否是空的
            if (fileSystemServices.File_Exists(this.GetConfigFilePath()))
            {
                return;
            }
            //如果不存在data文件夹，创建data文件夹
            fileSystemServices.Directory_CreateDirectory(DataDir);

            //如果不存在history文件夹，创建history文件夹
            fileSystemServices.Directory_CreateDirectory(HistoryDir);

            //如果不存在config文件夹，创建config文件夹
            fileSystemServices.Directory_CreateDirectory(ConfigDir);

      

            // 打开数据库 (如果不存在则创建)
            using (var db = new LiteDatabase(DataBase))
            {
                // 获得 customer 集合
                var col = db.GetCollection<Customer>("customers");

                // 创建你的新 customer 实例
                var customer = new Customer
                {
                    Name = "John Doe",
                    Phones = new string[] { "8000-0000", "9000-0000" },
                    Age = 39,
                    IsActive = true
                };

                // 在 Name 字段上创建唯一索引
                col.EnsureIndex(x => x.Name, true);

                // 插入新的 customer 文档 (Id 是自增的)
                col.Insert(customer);

                // 更新集合中的一个文档
                customer.Name = "Joana Doe";

                col.Update(customer);

                // 使用 LINQ 查询文档 (未使用索引)
                var results = col.Find(x => x.Age > 20);
            }
        }


    }
}