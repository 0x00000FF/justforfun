using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace Kiwi3Lang
{
    enum Direction : short
    {
        up = 38, down = 40, left = 37, right = 39
    }

    enum ItemType : int
    {
        firebase = 1, unity = 2, vuejs = 3
    }

    /*
     * TODO: 낄낄타임, 오징어, 서당개
     * 
     * 
     * 
     */ 

    static class Program
    {
        const short GAME_TICK = 5;
        const short ITEM_TICK = 120;
        const short KIWI_TICK = 20;

        static bool start = false;
        static bool stp1 = false;
        static bool stp2 = false;
        static bool stp3 = false;


        static bool dead = false;
        static short gametick = 0;
        static short kiwitick = 0;
        static short itemtick = 0;

        static byte firepower;

        class PowerItem
        {
            public Vector2f position;
            public Sprite sprite;
            public ItemType itemtype;

            public PowerItem(float x, float y, ItemType type)
            {
                position = new Vector2f(x, y);

                if (type == ItemType.firebase)
                    sprite = new Sprite(
                            new Texture("resources/firebase.png")
                            );
                else if (type == ItemType.unity)
                    sprite = new Sprite(
                            new Texture("resources/unity.png")
                        );
                else
                    sprite = new Sprite(
                            new Texture("resources/vuejs.png")
                        );

                itemtype = type;
            }
        }

        static List<Vector2f> kiwitrails = new List<Vector2f>();
        static List<PowerItem> poweritem = new List<PowerItem>();
        static List<Vector2f> kiwis = new List<Vector2f>();

        static Vector2f kiwiposition;
        static Direction kiwidirection;

        static int score = 0;

        static void Main()
        {
            var win = new RenderWindow(
                    new VideoMode(1200, 600),
                    "키위세개어 게임"
                );
            win.SetFramerateLimit(60);

            win.KeyPressed += (sender, args) =>
            {
                if (!start) start = true;
                if (args.Code == Keyboard.Key.X)
                {
                    if (firepower == 100)
                    {
                        poweritem.Clear();

                        score += kiwis.Count * 10;
                        kiwis.Clear();

                        firepower = 0;
                    }

                    return;
                }
                else if (args.Code == Keyboard.Key.R)
                {
                    firepower = 0;
                    score = 0;

                    kiwiposition = new Vector2f(15, 10);
                    kiwidirection = Direction.right;
                    dead = false;

                    kiwis.Clear();
                    kiwitrails.Clear();
                    poweritem.Clear();
                }

                switch (args.Code)
                {
                    case Keyboard.Key.Left:
                        if (kiwidirection == Direction.right) return;
                        else kiwidirection = Direction.left;
                        break;

                    case Keyboard.Key.Right:
                        if (kiwidirection == Direction.left) return;
                        else kiwidirection = Direction.right;
                        break;

                    case Keyboard.Key.Up:
                        if (kiwidirection == Direction.down) return;
                        else kiwidirection = Direction.up;
                        break;

                    case Keyboard.Key.Down:
                        if (kiwidirection == Direction.up) return;
                        else kiwidirection = Direction.down;
                        break;
                }
            };

            win.Closed += (sender, args) =>
            {
                win.Close();
            };


            // PREPARE GAME SPRITES
            var FieldBlock = new RectangleShape(new Vector2f(30, 30))
            {
                FillColor = Color.White,
                OutlineColor = Color.Black,
                OutlineThickness = 1.0f
            };

            var BorderBlock = new RectangleShape(new Vector2f(30, 30))
            {
                FillColor = Color.Yellow,
                OutlineColor = Color.Black,
                OutlineThickness = 1.0f
            };


            var FirePowerStatus = new Text("땔깜게이지", new Font("resources/kpdl.ttf"))
            {
                CharacterSize = 26,
                Position = new Vector2f(930, 30)
            };

            var FirePowerGuage = new RectangleShape(new Vector2f(0, 30))
            {
                FillColor = Color.Red,
                Position = new Vector2f(930, 70)
            };

            var Score = new Text("점수", new Font("resources/kpdl.ttf"))
            {
                CharacterSize = 26,
                Position = new Vector2f(930, 110)
            };

            var ScoreText = new Text("", new Font("resources/kpdl.ttf"))
            {
                CharacterSize = 26,
                Position = new Vector2f(930, 150)
            };

            var Status = new Text("상태", new Font("resources/kpdl.ttf"))
            {
                CharacterSize = 26,
                Position = new Vector2f(930, 190)
            };

            var StatusText = new Text("정상", new Font("resources/kpdl.ttf"))
            {
                CharacterSize = 26,
                Position = new Vector2f(930, 230)
            };

            var KiwiHeadTexture = new Texture("resources/kiwi_head.png");
            var KiwiHead = new Sprite(KiwiHeadTexture);

            var KiwiTexture = new Texture("resources/kiwi_item.png");
            var Kiwi = new Sprite(KiwiTexture);

            var Start = new Text("Press Anykey to Start", new Font("resources/kpdl.ttf"))
            {
                CharacterSize = 50,
                Color = Color.Red,
                Position = new Vector2f(30, 30)
            };

            var DeadTexture = new Texture("resources/dead.png");
            var Dead = new Sprite(DeadTexture);

            // RESET GAME PARAMETERS
            firepower = 0;
            kiwiposition = new Vector2f(15, 10);
            kiwidirection = Direction.right;

            while (win.IsOpen)
            {
                win.DispatchEvents();

                if (start)
                {
                    // UPDATE GAME STATE
                    if (gametick >= GAME_TICK && !dead)
                    {
                        for (var k = kiwitrails.Count - 1; k > 0; --k)
                        {
                            kiwitrails[k] = kiwitrails[k - 1];
                        }

                        if (kiwitrails.Count != 0)
                        kiwitrails[0] = kiwiposition;

                        switch (kiwidirection)
                        {
                            case Direction.up:
                                kiwiposition.Y -= 1;
                                break;
                            case Direction.down:
                                kiwiposition.Y += 1;
                                break;
                            case Direction.left:
                                kiwiposition.X -= 1;
                                break;
                            case Direction.right:
                                kiwiposition.X += 1;
                                break;
                        }

                        if (itemtick >= ITEM_TICK && kiwitrails.Count == 3)
                        {
                            // CREATE ITEM ON FIELD
                            bool regen;

                            do
                            {
                                regen = false;
                                float randX, randY;
                                ItemType randItem;

                                var rand = new Random();
                                randX = rand.Next(1, 28);
                                randY = rand.Next(1, 18);
                                randItem = (ItemType)rand.Next(1, 3);

                                var newitem = new PowerItem(randX, randY, randItem);

                                if (!kiwis.Contains(newitem.position))
                                {
                                    foreach (var item in poweritem)
                                    {
                                        if (item.position == newitem.position)
                                        {
                                            regen = true;
                                            break;
                                        }
                                    }

                                    if (!regen) poweritem.Add(newitem);
                                }
                                else
                                {
                                    continue;
                                }
                            } while (regen);

                            itemtick = 0;
                        }

                        if (kiwis.Count == 0 || kiwitick >= KIWI_TICK)
                        {
                            // CREATE KIWI ON FIELD
                            if (kiwitrails.Count == 3 || kiwis.Count == 0)
                            {
                                bool regen;

                                do
                                {
                                    regen = false;
                                    float randX, randY;

                                    var rand = new Random();
                                    randX = rand.Next(1, 28);
                                    randY = rand.Next(1, 18);

                                    var newkiwi = new Vector2f(randX, randY);
                                    if (!kiwis.Contains(newkiwi))
                                    {
                                        foreach (var item in poweritem)
                                        {
                                            if (item.position == newkiwi)
                                            {
                                                regen = true;
                                                break;
                                            }
                                        }

                                        if (!regen) kiwis.Add(newkiwi);
                                    }

                                } while (regen);
                            }

                            kiwitick = 0;
                        }

                        score++;
                        gametick = 0;
                    }

                    gametick++;
                    itemtick++;
                    kiwitick++;

                    // CHECK COLLISION
                    if (kiwiposition.Y == 0 || kiwiposition.Y == 19 || kiwiposition.X == 0 || kiwiposition.X == 29)
                        dead = true;
                    
                    foreach (var kiwi in kiwis.ToArray())
                    {
                        if (kiwiposition.X == kiwi.X && kiwiposition.Y == kiwi.Y)
                        {
                            if (kiwitrails.Count == 3)
                            {
                                dead = true;
                            }
                            else
                            {
                                score += 50;

                                var newkiwi = new Vector2f();
                                switch (kiwidirection)
                                {
                                    case Direction.up:
                                        if (kiwitrails.Count == 0 )
                                        {
                                            newkiwi.X = KiwiHead.Position.X;
                                            newkiwi.Y = KiwiHead.Position.Y + 1;
                                        }
                                        else
                                        {
                                            newkiwi.X = kiwitrails[kiwitrails.Count - 1].X;
                                            newkiwi.Y = kiwitrails[kiwitrails.Count - 1].Y + 1;
                                        }
                                        break;

                                    case Direction.down:
                                        if (kiwitrails.Count == 0)
                                        {
                                            newkiwi.X = KiwiHead.Position.X;
                                            newkiwi.Y = KiwiHead.Position.Y - 1;
                                        }
                                        else
                                        {
                                            newkiwi.X = kiwitrails[kiwitrails.Count - 1].X;
                                            newkiwi.Y = kiwitrails[kiwitrails.Count - 1].Y - 1;
                                        }
                                        break;

                                    case Direction.left:
                                        if (kiwitrails.Count == 0)
                                        {
                                            newkiwi.X = KiwiHead.Position.X + 1;
                                            newkiwi.Y = KiwiHead.Position.Y;
                                        }
                                        else
                                        {
                                            newkiwi.X = kiwitrails[kiwitrails.Count - 1].X + 1;
                                            newkiwi.Y = kiwitrails[kiwitrails.Count - 1].Y;
                                        }
                                        break;

                                    case Direction.right:
                                        if (kiwitrails.Count == 0)
                                        {
                                            newkiwi.X = KiwiHead.Position.X - 1;
                                            newkiwi.Y = KiwiHead.Position.Y;
                                        }
                                        else
                                        {
                                            newkiwi.X = kiwitrails[kiwitrails.Count - 1].X - 1;
                                            newkiwi.Y = kiwitrails[kiwitrails.Count - 1].Y;
                                        }
                                        break;
                                }

                                kiwitrails.Add(newkiwi);

                                kiwis.Remove(kiwi);
                                break;
                            }
                        }
                    }

                    foreach (var item in poweritem.ToArray())
                    {
                        if (kiwiposition.X == item.position.X && kiwiposition.Y == item.position.Y)
                        {
                            switch (item.itemtype)
                            {
                                case ItemType.firebase:
                                    score += 50;
                                    firepower += 20;
                                    break;

                                case ItemType.unity:
                                    score += 70;
                                    firepower += 30;
                                    break;

                                case ItemType.vuejs:
                                    score += 100;
                                    firepower += 50;
                                    break;
                            }

                            poweritem.Remove(item);
                            break;
                        }
                    }

                }

                if (firepower > 100)
                    firepower = 100;

                if (score >= 1000 && !stp1)
                {
                    stp1 = true;
                    gametick--;
                }

                if (score >= 2500 && !stp2)
                {
                    stp2 = true;
                    gametick--;
                }

                if (score >= 5000 && !stp3)
                {
                    stp3 = true;
                    gametick--;
                }

                // REDRAW GAME SCREEN
                win.Clear();

                for (var x = 0; x < 30; ++x)
                    for (var y = 0; y < 20; ++y)
                    {
                        Drawable drawrect;

                        if (y == 0 || y == 19 || x == 0 || x == 29)
                        {
                            BorderBlock.Position = new Vector2f(x * 30, y * 30);
                            drawrect = BorderBlock;
                        }
                        else
                        {
                            FieldBlock.Position = new Vector2f(x * 30, y * 30);
                            drawrect = FieldBlock;
                        }

                        win.Draw(drawrect);
                    }

                if (start)
                {
                    // DRAW KIWI
                    KiwiHead.Position = new Vector2f(kiwiposition.X * 30, kiwiposition.Y * 30);
                    win.Draw(KiwiHead);

                    foreach (var kiwi in kiwitrails)
                    {
                        Kiwi.Position = new Vector2f(kiwi.X * 30, kiwi.Y * 30);
                        win.Draw(Kiwi);
                    }
                }
                else
                {
                    win.Draw(Start);
                }

                foreach (var kiwipos in kiwis)
                {
                    Kiwi.Position = new Vector2f(kiwipos.X * 30, kiwipos.Y * 30);
                    win.Draw(Kiwi);
                }

                foreach (var item in poweritem)
                {
                    item.sprite.Position = new Vector2f(item.position.X * 30, item.position.Y * 30);
                    win.Draw(item.sprite);
                }

                win.Draw(FirePowerStatus);

                FirePowerGuage.Size = new Vector2f(firepower * 2.3f, 30);
                FirePowerGuage.FillColor = firepower == 100 ? Color.Blue : Color.Red;
                win.Draw(FirePowerGuage);

                win.Draw(Score);

                ScoreText.DisplayedString = score.ToString();
                win.Draw(ScoreText);

                win.Draw(Status);
                if (dead)
                {
                    StatusText.DisplayedString = "사망";
                    StatusText.Color = Color.Red;

                    Dead.Position = new Vector2f(kiwiposition.X * 30, kiwiposition.Y * 30);
                    win.Draw(Dead);
                }
                else
                {
                    StatusText.DisplayedString = "정상";
                    StatusText.Color = Color.Blue;
                }
                win.Draw(StatusText);

                win.Display();


            }
        }
    }
}
