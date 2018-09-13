using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class DBHandler
{
	public static string Error {get; private set;}

	public static bool AccountConnection(string login, string mdp)
	{
		IDataReader reader = DBManager.Select("login", "Account", "login='" + login + "' AND password='" + mdp + "'");
		if (reader.Read())
		{
			reader.Close();
			reader = null;
			return true;
		}
		Error = "Erreur login ou mot de passe!";
		reader.Close();
		reader = null;
		return false;
	}

	public static bool AccountExist(string login)
	{
		IDataReader reader = DBManager.Select("login", "Account", "login='" + login + "'");
		if (reader.Read())
		{
			reader.Close();
			reader = null;
			return true;
		}
		reader.Close();
		reader = null;
		return false;
	}

	public static bool AccountRegister(string login, string mdp)
	{
		if (login.Length >= 4)
		{
			if (mdp.Length >= 6)
			{
				if (AccountExist(login) == false)
				{
					DBManager.Insert("Account", "login, password", "'" + login + "', '" + mdp + "'");
					return true;
				}
				else
					Error = "Le Login exist deja!";
			}
			else
				Error = "Le mot de passe doit faire 6 caractere au minimum";
		}
		else
			Error = "Le login doit faire 4 caractere au minimum";
		return false;
	}

	public static bool CharacterExist(string name)
	{
		IDataReader reader = DBManager.Select("name", "Character", "name='" + name + "'");
		if (reader.Read())
		{
			reader.Close();
			reader = null;
			return true;
		}
		reader.Close();
		reader = null;
		return false;
	}

	public static int CharacterAdd(int account, string name, string sprite)
	{
		DBManager.Insert("Character",
			"account, name, level, exp, skilltree, life, sprite",
			account + ", '" + name + "', 1, 0, '', 50, '" + sprite + "'"
		);
		IDataReader reader = DBManager.Select("id", "Character", "name='" + name + "'");
		int id = -1;
		if (reader.Read())
			id = reader.GetInt32(0);
		reader.Close();
		reader = null;
		return id;
	} 
}