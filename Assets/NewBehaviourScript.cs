using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMove : MonoBehaviour // - Вместо «PlayerMove» должно быть имя файла скрипта
{
//------- Функция/метод, выполняемая при запуске игры ---------
	public Rigidbody2D rb;
	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
	}
//------- Функция/метод, выполняемая каждый кадр в игре ---------
	void Update()
	{
		Walk();
	}
//------- Функция/метод для перемещения персонажа по горизонтали ---------
	public float speed = 2f;
	public Vector2 moveVector;
	void Walk ()
	{
		moveVector.x = Input.GetAxisRaw("Horizontal");
		rb.velocity = new Vector2(moveVector.x * speed, rb.velocity.y);
	}
//-----------------------------------------------------------------
}