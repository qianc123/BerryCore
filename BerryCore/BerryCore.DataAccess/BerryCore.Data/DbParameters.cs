﻿#region << 版 本 注 释 >>

/*----------------------------------------------------------------
* 项目名称 ：BerryCore.Data
* 项目描述 ：
* 类 名 称 ：DbParameters
* 类 描 述 ：
* 所在的域 ：MRZHAOYI
* 命名空间 ：BerryCore.Data
* 机器名称 ：MRZHAOYI
* CLR 版本 ：4.0.30319.42000
* 作    者 ：赵轶
* 创建时间 ：2019/5/3 22:28:17
* 更新时间 ：2019/5/3 22:28:17
* 版 本 号 ：V1.0.0.0
***********************************************************************
* Copyright © 大師兄丶 2019. All rights reserved.                     *
***********************************************************************
//----------------------------------------------------------------*/

#endregion << 版 本 注 释 >>

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Reflection;
using MySql.Data.MySqlClient;
using Oracle.DataAccess.Client;

namespace BerryCore.Data
{
    /// <summary>
    /// 功能描述    ：SQL参数化
    /// 创 建 者    ：赵轶
    /// 创建日期    ：2019/5/3 22:28:17
    /// 最后修改者  ：赵轶
    /// 最后修改日期：2019/5/3 22:28:17
    /// </summary>
    public static class DbParameters
    {
        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// </summary>
        /// <returns></returns>
        public static string CreateDbParmCharacter()
        {
            string character;
            switch (DbTypeContainer.DbType)
            {
                case DatabaseType.SqlServer:
                    character = "@";
                    break;

                case DatabaseType.Oracle:
                    character = ":";
                    break;

                case DatabaseType.MySql:
                    character = "?";
                    break;

                case DatabaseType.SQLite:
                    character = "@";
                    break;

                default:
                    throw new Exception("数据库类型目前不支持！");
            }
            return character;
        }

        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter()
        {
            DbParameter parameter;
            switch (DbTypeContainer.DbType)
            {
                case DatabaseType.SqlServer:
                    parameter = new SqlParameter();
                    break;

                case DatabaseType.Oracle:
                    parameter = new OracleParameter();
                    break;

                case DatabaseType.MySql:
                    parameter = new MySqlParameter();
                    break;

                case DatabaseType.SQLite:
                    parameter = new SQLiteParameter();
                    break;

                default:
                    parameter = new SqlParameter();
                    break;
            }
            return parameter;
        }

        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(string paramName, object value)
        {
            DbParameter param = DbParameters.CreateDbParameter();
            param.ParameterName = paramName;
            param.Value = value;
            return param;
        }

        /// <summary>
        /// 根据配置文件中所配置的数据库类型
        /// 来创建相应数据库的参数对象
        /// </summary>
        /// <returns></returns>
        public static DbParameter CreateDbParameter(string paramName, object value, DbType dbType)
        {
            DbParameter param = DbParameters.CreateDbParameter();
            param.DbType = dbType;
            param.ParameterName = paramName;
            param.Value = value;
            return param;
        }

        /// <summary>
        /// 转换对应的数据库参数
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static DbParameter[] ToDbParameter(DbParameter[] parameter)
        {
            int i = 0;
            int size = parameter.Length;
            DbParameter[] dbParameter = null;
            switch (DbTypeContainer.DbType)
            {
                case DatabaseType.SqlServer:
                    dbParameter = new DbParameter[size];
                    while (i < size)
                    {
                        dbParameter[i] = new SqlParameter(parameter[i].ParameterName, parameter[i].Value);
                        i++;
                    }
                    break;
                case DatabaseType.MySql:
                    dbParameter = new DbParameter[size];
                    while (i < size)
                    {
                        dbParameter[i] = new MySqlParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                        i++;
                    }
                    break;
                case DatabaseType.Oracle:
                    dbParameter = new DbParameter[size];
                    while (i < size)
                    {
                        dbParameter[i] = new OracleParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                        i++;
                    }
                    break;
                case DatabaseType.SQLite:
                    dbParameter = new DbParameter[size];
                    while (i < size)
                    {
                        dbParameter[i] = new SQLiteParameter(dbParameter[i].ParameterName, dbParameter[i].Value);
                        i++;
                    }
                    break;
                default:
                    throw new Exception("数据库类型目前不支持！");
            }
            return dbParameter;
        }

        /// <summary>
        /// DbParameter转成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbParameter"></param>
        /// <returns></returns>
        public static T DbParameterToObject<T>(this DbParameter[] dbParameter) where T : new()
        {
            T t = new T();
            if (dbParameter != null)
            {
                Type type = typeof(T);
                PropertyInfo[] props = type.GetProperties();

                foreach (DbParameter parameter in dbParameter)
                {
                    if (parameter.Value != null)
                    {
                        foreach (PropertyInfo prop in props)
                        {
                            // 判断此属性是否有Setter
                            if (!prop.CanWrite)
                            {
                                continue;//该属性不可写，直接跳出
                            }

                            if (prop.Name == parameter.ParameterName.Replace(DbParameters.CreateDbParmCharacter(), ""))
                            {
                                prop.SetValue(t, parameter.Value, null);
                            }
                        }
                    }
                }
            }
            return t;
        }
    }
}