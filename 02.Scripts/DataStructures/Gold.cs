using System;
using System.Text;

namespace HTH.DataStructures
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 10^35 자릿수 까지 표현 가능한 골드. 추가 자릿수 필요시 클래스 전환 및 멤버 추가해야함.
    /// </summary>
    #pragma warning disable 0659
    #pragma warning disable 0661
    [Serializable]
    public struct Gold
    {
        public static Gold max => new Gold()
        {
            tsp0 = 999999999,
            tsp1 = 999999999,
            tsp2 = 999999999,
            tsp3 = 999999999,
        };

        public static Gold min => new Gold()
        {
            tsp0 = 0,
            tsp1 = 0,
            tsp2 = 0,
            tsp3 = 0,
        };

        public static Gold zero => new Gold()
        {
            tsp0 = 0,
            tsp1 = 0,
            tsp2 = 0,
            tsp3 = 0,
        };

        // tsp : ten to six power
        public int tsp0; // 0 to m (10^6)
        public int tsp1; // g(10^9) to p(10^15)
        public int tsp2; // e(10^18) to y (10^24)
        public int tsp3; // r(10^27) to ak (10^33)
        private StringBuilder stringBuilder { get; set; }


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public Gold(int tnp0, int tnp1, int tnp2, int tnp3)
        {
            this.tsp0 = tnp0;
            this.tsp1 = tnp1;
            this.tsp2 = tnp2;
            this.tsp3 = tnp3;
            stringBuilder = new StringBuilder();
        }

        public string GetSimplifiedString()
        {
            if (stringBuilder == null)
                stringBuilder = new StringBuilder();

            stringBuilder.Clear();

            if (tsp3 / 1000000 > 0) stringBuilder.Append(tsp3 / 1000000).Append('.').Append(tsp3 % 1000000 / 100000).Append("ak");
            else if (tsp3 / 1000 > 0) stringBuilder.Append(tsp3 / 1000).Append('.').Append(tsp3 % 1000 / 100).Append("q");
            else if (tsp3 / 1 > 0) stringBuilder.Append(tsp3 / 1).Append('.').Append(tsp2 / 100000000).Append("r");

            else if (tsp2 / 1000000 > 0) stringBuilder.Append(tsp2 / 1000000).Append('.').Append(tsp2 % 1000000 / 100000).Append("y");
            else if (tsp2 / 1000 > 0) stringBuilder.Append(tsp2 / 1000).Append('.').Append(tsp2 % 1000 / 100).Append("z");
            else if (tsp2 / 1 > 0) stringBuilder.Append(tsp2 / 1).Append('.').Append(tsp1 / 100000000).Append("e");

            else if (tsp1 / 1000000 > 0) stringBuilder.Append(tsp1 / 1000000).Append('.').Append(tsp1 % 1000000 / 100000).Append("p");
            else if (tsp1 / 1000 > 0) stringBuilder.Append(tsp1 / 1000).Append('.').Append(tsp1 % 1000 / 100).Append("t");
            else if (tsp1 / 1 > 0) stringBuilder.Append(tsp1 / 1).Append('.').Append(tsp0 / 100000000).Append("g");

            else if (tsp0 / 1000000 > 0) stringBuilder.Append(tsp0 / 1000000).Append('.').Append(tsp0 % 1000000 / 100000).Append("m");
            else if (tsp0 / 1000 > 0) stringBuilder.Append(tsp0 / 1000).Append('.').Append(tsp0 % 1000 / 100).Append("k");
            else if (tsp0 / 1 > 0) stringBuilder.Append(tsp0 / 1);

            return stringBuilder.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is Gold gold &&
                   tsp0 == gold.tsp0 &&
                   tsp1 == gold.tsp1 &&
                   tsp2 == gold.tsp2 &&
                   tsp3 == gold.tsp3;
        }


        public static Gold operator +(Gold op1, Gold op2)
        {
            int temp0 = op1.tsp0 + op2.tsp0;
            int temp1 = op1.tsp1 + op2.tsp1 + temp0 / 1000000000;
            int temp2 = op1.tsp2 + op2.tsp2 + temp1 / 1000000000;
            int temp3 = op1.tsp3 + op2.tsp3 + temp2 / 1000000000;

            return new Gold()
            {
                tsp0 = temp0 % 1000000000,
                tsp1 = temp1 % 1000000000,
                tsp2 = temp2 % 1000000000,
                tsp3 = temp3 % 1000000000,
            };
        }

        public static Gold operator -(Gold op1, Gold op2)
        {
            int temp0, temp1, temp2, temp3;
            temp0 = op1.tsp0 - op2.tsp0;
            temp1 = op1.tsp1 - op2.tsp1;
            if (temp0 < 0)
            {
                temp1 -= 1;
                temp0 += 100000000;
            }

            temp2 = op1.tsp2 - op2.tsp2;
            if (temp1 < 0)
            {
                temp2 -= 1;
                temp1 += 100000000;
            }

            temp3 = op1.tsp3 - op2.tsp3;
            if (temp2 < 0)
            {
                temp3 -= 1;
                temp2 += 100000000;
            }

            return new Gold()
            {
                tsp0 = temp0,
                tsp1 = temp1,
                tsp2 = temp2,
                tsp3 = temp3,
            };
        }

        public static Gold operator *(Gold op1, int multiplier)
        {
            int temp0 = (op1.tsp0 * multiplier);
            int temp1 = (op1.tsp1 * multiplier) + temp0 / 1000000000;
            int temp2 = (op1.tsp2 * multiplier) + temp1 / 1000000000;
            int temp3 = (op1.tsp3 * multiplier) + temp2 / 1000000000;

            return new Gold()
            {
                tsp0 = temp0 % 1000000000,
                tsp1 = temp1 % 1000000000,
                tsp2 = temp2 % 1000000000,
                tsp3 = temp3 % 1000000000,
            };
        }

        public static Gold operator *(Gold op1, float multiplier)
        {
            float temp0 = (op1.tsp0 * multiplier);
            float temp1 = (op1.tsp1 * multiplier) + temp0 / 1000000000;
            float temp2 = (op1.tsp2 * multiplier) + temp1 / 1000000000;
            float temp3 = (op1.tsp3 * multiplier) + temp2 / 1000000000;

            return new Gold()
            {
                tsp0 = (int)temp0 % 1000000000,
                tsp1 = (int)temp1 % 1000000000,
                tsp2 = (int)temp2 % 1000000000,
                tsp3 = (int)temp3 % 1000000000,
            };
        }

        public static bool operator ==(Gold op1, Gold op2)
        {
            return (op1.tsp3 == op2.tsp3) && (op1.tsp2 == op2.tsp2) && (op1.tsp1 == op2.tsp1) && (op1.tsp0 == op2.tsp0);
        }

        public static bool operator !=(Gold op1, Gold op2)
            => !(op1 == op2);

        public static bool operator <(Gold op1, Gold op2)
        {
            if (op1.tsp3 > op2.tsp3 ||
                op1.tsp2 > op2.tsp2 ||
                op1.tsp1 > op2.tsp1 ||
                op1.tsp0 > op2.tsp0 ||
                op1 == op2)
                return false;
            else
                return true;
        }

        public static bool operator <=(Gold op1, Gold op2)
        {
            if (op1.tsp3 > op2.tsp3 ||
                op1.tsp2 > op2.tsp2 ||
                op1.tsp1 > op2.tsp1 ||
                op1.tsp0 > op2.tsp0)
                return false;
            else
                return true;
        }

        public static bool operator >(Gold op1, Gold op2)
        {
            if (op1.tsp3 < op2.tsp3 ||
                op1.tsp2 < op2.tsp2 ||
                op1.tsp1 < op2.tsp1 ||
                op1.tsp0 < op2.tsp0 ||
                op1 == op2)
                return false;
            else
                return true;
        }

        public static bool operator >=(Gold op1, Gold op2)
        {
            if (op1.tsp3 < op2.tsp3 ||
                op1.tsp2 < op2.tsp2 ||
                op1.tsp1 < op2.tsp1 ||
                op1.tsp0 < op2.tsp0)
                return false;
            else
                return true;
        }
    }
}