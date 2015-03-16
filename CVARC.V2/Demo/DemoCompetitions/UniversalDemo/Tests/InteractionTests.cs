﻿using AIRLab.Mathematics;
using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo
{
	partial class DemoLogicPartHelper
	{

        

		void LoadInteractionTests(LogicPart logic, MoveAndGripRules rules)
		{

			//выравнивание при ударении о стену (проверяется на location)
			logic.Tests["Interaction_Rect_Alignment"] = new RectangularInteractionTestBase(
				LocationTest((frame,asserter)=>
					{
						asserter.IsEqual(Angle.HalfPi.Grad,frame.Angle.Grad,1);
						asserter.IsEqual(22.36, frame.Y, 1e-3);
					},
				rules.Move(-10),
				rules.Rotate(Angle.HalfPi / 2),
				rules.Move(50)));

            logic.Tests["Interaction_Round_Alignment"] = new RoundInteractionTestBase(
                LocationTest((frame, asserter) => asserter.IsEqual(Angle.HalfPi.Grad/2, frame.Angle.Grad, 1),
                rules.Rotate(Angle.HalfPi / 2),
                rules.Move(30)));

            logic.Tests["Interaction_Round_Alignment2"] = new RoundInteractionTestBase(
                LocationTest((frame, asserter) =>
                {
                    asserter.IsEqual(Angle.HalfPi.Grad, frame.Angle.Grad, 1);
                    asserter.IsEqual(17.45, frame.Y, 1e-3);
                },
                rules.Rotate(Angle.HalfPi / 2),
                rules.Move(30),
                rules.Rotate(Angle.HalfPi / 2),
                rules.Rotate(Angle.Pi*2),
                rules.Move(10)));



			logic.Tests["Interaction_Rect_Collision"] = new RectangularInteractionTestBase(LocationTest(
			   (frame, asserter) => asserter.True(frame.X < 100 && frame.X > 70),
			   rules.Move(100))); 

            
		}
	}
}		

