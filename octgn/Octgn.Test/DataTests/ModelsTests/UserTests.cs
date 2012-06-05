﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Db4objects.Db4o;
//using Db4objects.Db4o.Query;
using FakeItEasy;
using System.Web.Security;
using NUnit.Framework;
using Octgn.Data;
using Octgn.Data.Models;

namespace Octgn.Test.DataTests.ModelsTests
{
	public class UserTests
	{
		[Test]
		public void CreateUserTest()
		{
			var f = A.Fake<Db4objects.Db4o.IObjectContainer>();
			var fList = new List<User>();

			var db = new Database(f);
			var user = new User(db);
			user.Email = "fake@email.com";
			user.Username = "fakeuser";
			user.PasswordHash = "123456";

			A.CallTo(() => db.DBConnection.Store(new object())).WithAnyArguments().DoesNothing();
			A.CallTo(() => db.DBConnection.Query<User>()).Returns(fList);

			fList.Add(user);

			MembershipCreateStatus ret = MembershipCreateStatus.ProviderError;

			//Test with non conflicting user
			ret = user.CreateUser("nuser", "npass", "nemail");
			Assert.AreEqual(ret, MembershipCreateStatus.Success);

			//Test with Conflicting username
			ret = user.CreateUser("fakeuser", "pass", "email");
			Assert.AreEqual(ret, MembershipCreateStatus.DuplicateUserName);

			//Test with conflicting email
			ret = user.CreateUser("nuser", "pass", "fake@email.com");
			Assert.AreEqual(ret, MembershipCreateStatus.DuplicateEmail);
		}
		[Test]
		public void ValidateUserTest()
		{
			var f = A.Fake<Db4objects.Db4o.IObjectContainer>();
			var fList = new List<User>();

			var db = new Database(f);
			var user = new User(db);
			user.Email = "fake@email.com";
			user.Username = "fakeuser";
			user.PasswordHash = "123456";

			A.CallTo(() => db.DBConnection.Store(new object())).WithAnyArguments().DoesNothing();
			A.CallTo(() => db.DBConnection.Query<User>()).Returns(fList);

			fList.Add(user);

			//Validate good username & password
			Assert.IsTrue(user.ValidateUser("fakeuser" , "123456"));

			//Validate good username & bad password
			Assert.IsFalse(user.ValidateUser("fakeuser", "BAD PASS"));

			//Validate bad username & good password
			Assert.IsFalse(user.ValidateUser("Bad Username", "123456"));

		}
	}
}