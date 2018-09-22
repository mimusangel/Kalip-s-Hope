using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public static class DBManager {

	private static SqliteConnection dbconn = null;
	private static IDbTransaction transaction = null;

	public static void OpenDataBase()
	{
		if (dbconn != null)
			return ;
		string conn = "URI=file:" + Application.streamingAssetsPath + "/GameData.db";
		dbconn = new SqliteConnection(conn);
		dbconn.Open();
		Debug.Log("Database is linked!");
	}
	public static void CloseDataBase()
	{
		if (dbconn == null)
			return ;
		dbconn.Close();
		dbconn = null;
	}

	public static void Insert(string table, string columns, string values)
	{
		if (dbconn == null)
			OpenDataBase();
		IDbCommand cmd = dbconn.CreateCommand();
		cmd.CommandText = "INSERT INTO " + table + " (" + columns + ") VALUES (" + values + ")";
		Debug.Log(cmd.CommandText);
		if (transaction != null)
			cmd.ExecuteNonQuery();
		else
		{
			IDataReader reader = cmd.ExecuteReader();
			reader.Close();
		}
		cmd.Dispose();
		cmd = null;
	}

	public static IDataReader Select(string columns, string table) {
		if (dbconn == null)
			OpenDataBase();
		IDbCommand cmd = dbconn.CreateCommand();
		cmd.CommandText = "SELECT " + columns + " FROM " + table;
		Debug.Log(cmd.CommandText);
		IDataReader reader = cmd.ExecuteReader();
		cmd.Dispose();
		cmd = null;
		return (reader);
	}

	public static IDataReader Select(string columns, string table, string where) {
		if (dbconn == null)
			OpenDataBase();
		IDbCommand cmd = dbconn.CreateCommand();
		cmd.CommandText = "SELECT " + columns + " FROM " + table + " WHERE " + where;
		Debug.Log(cmd.CommandText);
		IDataReader reader = cmd.ExecuteReader();
		cmd.Dispose();
		cmd = null;
		return (reader);
	}

	public static void Update(string table, string values, string where) {
		if (dbconn == null)
			OpenDataBase();
		IDbCommand cmd = dbconn.CreateCommand();
		cmd.CommandText = "UPDATE " + table + " SET " + values + " WHERE " + where;
		Debug.Log(cmd.CommandText);
		if (transaction != null)
			cmd.ExecuteNonQuery();
		else
		{
			IDataReader reader = cmd.ExecuteReader();
			reader.Close();
		}
		cmd.Dispose();
		cmd = null;
	}

	public static void Replace(string table, string columns, string values)
	{
		if (dbconn == null)
			OpenDataBase();
		IDbCommand cmd = dbconn.CreateCommand();
		cmd.CommandText = "REPLACE INTO " + table + " (" + columns + ") VALUES (" + values + ")";
		Debug.Log(cmd.CommandText);
		if (transaction != null)
			cmd.ExecuteNonQuery();
		else
		{
			IDataReader reader = cmd.ExecuteReader();
			reader.Close();
		}
		cmd.Dispose();
		cmd = null;
	}

	public static void Delete(string table, string where)
	{
		if (dbconn == null)
			OpenDataBase();
		IDbCommand cmd = dbconn.CreateCommand();
		cmd.CommandText = "DELETE FROM " + table + " WHERE " + where;
		Debug.Log(cmd.CommandText);
		if (transaction != null)
			cmd.ExecuteNonQuery();
		else
		{
			IDataReader reader = cmd.ExecuteReader();
			reader.Close();
		}
		cmd.Dispose();
		cmd = null;
	}

	public static void Prepare()
	{
		if (dbconn == null)
			OpenDataBase();
		if (transaction == null)
			transaction = dbconn.BeginTransaction();
	}

	public static void Commit()
	{
		 if (transaction != null)
		 {
            transaction.Commit();
			transaction.Dispose();
		 }
        transaction = null;
	}

	public static int LastInsertID(string table) {
		if (dbconn == null)
			OpenDataBase();
		IDbCommand cmd = dbconn.CreateCommand();
		cmd.CommandText = "SELECT last_insert_rowid() from " + table;
		Debug.Log(cmd.CommandText);
		long id = (long)cmd.ExecuteScalar();
		cmd.Dispose();
		cmd = null;
		return ((int)id);
	}
}
