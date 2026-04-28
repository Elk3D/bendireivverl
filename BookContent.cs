using System;
using UnityEngine;

public class BookContent : ActionEventContent<BookContent.Properties>
{
	[Serializable]
	public class Properties : ActionEventProperties
	{
		public Interactable BookContent_Fishing;

		public Interactable BookContent_Cooking;

		public Interactable BookContent_Spaces;

		public Interactable BookContent_Mug;
	}

	private Book m_Book;

	private UIBookDataVO m_DataVO;

	protected override void OnInitialize()
	{
		m_Book = (Book)base.Connectable;
		InitializeBookData();
		if (m_Book.Data != null && m_Properties != null && m_Properties.Length != 0)
		{
			Properties properties = m_Properties[0];
			properties.ActionEvent = GenerateBook(properties);
			if (!(properties.ActionEvent == null))
			{
				properties.ActionEvent.transform.SetParent(base.transform);
				properties.ActionEvent.transform.localPosition = Vector3.zero;
				properties.ActionEvent.transform.localEulerAngles = Vector3.zero;
				properties.ActionEvent.transform.localScale = Vector3.one;
			}
		}
	}

	private void InitializeBookData()
	{
		string text = m_Book.Data.ID.ToString();
		if (m_Book.Data.BookType == BookType.MugAndTheMaiden)
		{
			string[] bookPages = new string[9]
			{
				TextUtility.GetKey(text + "_P01"),
				TextUtility.GetKey(text + "_P02"),
				TextUtility.GetKey(text + "_P03"),
				TextUtility.GetKey(text + "_P04"),
				TextUtility.GetKey(text + "_P05"),
				TextUtility.GetKey(text + "_P06"),
				TextUtility.GetKey(text + "_P07"),
				TextUtility.GetKey(text + "_P08"),
				TextUtility.GetKey(text + "_P09")
			};
			m_DataVO = new UIBookDataVO(m_Book, m_Book.Data.BookType, bookPages);
		}
		else
		{
			m_DataVO = new UIBookDataVO(m_Book, m_Book.Data.BookType, TextUtility.GetKey(text));
		}
	}

	private Interactable GenerateBook(Properties properties)
	{
		return m_DataVO.BookType switch
		{
			BookType.FishGuide => CreateBook(properties.BookContent_Fishing), 
			BookType.CookingForEveryday => CreateBook(properties.BookContent_Cooking), 
			BookType.TheoryofSpaces => CreateBook(properties.BookContent_Spaces), 
			BookType.MugAndTheMaiden => CreateBook(properties.BookContent_Mug), 
			_ => null, 
		};
	}

	private Interactable CreateBook(Interactable interactable)
	{
		return GameManager.Instance.AssetManager.CreateAsset<Interactable>(interactable);
	}

	protected override void OnActivate()
	{
		InitializeBookData();
		GameManager.Instance.ShowInteraction(TextUtility.GetKey(InteractionType.INTERACTION_EXIT.ToString()));
		GameManager.Instance.ShowBook(m_DataVO);
	}

	protected override void OnDeactivate()
	{
		DeactivateComplete();
	}

	protected override void OnDisposed()
	{
		m_Book = null;
		m_DataVO = null;
		base.OnDisposed();
	}
}
