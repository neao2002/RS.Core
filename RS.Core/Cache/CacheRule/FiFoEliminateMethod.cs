using System.Collections;
using System;

namespace RS.Core.Cache
{
	/// <summary>
	/// �Ƚ��ȳ�����̭�㷨
    /// </summary>
    [Serializable]
    public class FiFoEliminateMethod:IEliminateMethod
	{
		#region IEliminateMethod ��Ա

		private ArrayList MapOfWeights;

		public FiFoEliminateMethod()
		{
           MapOfWeights=new ArrayList();
		}

		/// <summary>
		/// ɾ�����Ƚ��뻺���Ȩֵ
		/// </summary>
		public void DelPoorWeight()
		{
			// TODO:  ��� FiFoEliminateMethod.DelPoorWeight ʵ��
			if(MapOfWeights.Count==0)return;
			lock(MapOfWeights)
			{
				MapOfWeights.Remove(MapOfWeights[0]);
			}
		}

		/// <summary>
		/// ɾ�������е�ĳ��Ȩֵ
		/// </summary>
		/// <param name="id"></param>
		public void DelWeight(object id)
		{
			lock(MapOfWeights)
			{
              MapOfWeights.Remove(id);
			}

		}
		/// <summary>
		/// ���Ȩֵ��С�����ID
		/// </summary>
		/// <returns></returns>
		public object GetPoorWeightID()
		{
			// TODO:  ��� FiFoEliminateMethod.GetPoorWeightID ʵ��
			lock(MapOfWeights)
			{
				if(MapOfWeights.Count==0)return null;
				return  MapOfWeights[0];
			}
		}

		/// <summary>
		/// ���ĳ���Ӧ��Ȩֵ
		/// </summary>
		/// <param name="id"></param>
		public void InsertWeight(object id)
		{
			// TODO:  ��� FiFoEliminateMethod.InsertWeight ʵ��
			lock(MapOfWeights)
			{
				MapOfWeights.Add(id);
			}
		}

		/// <summary>
		/// �޸�Ȩֵ:����Ȩֵ������󣬼�������뻺��
		/// </summary>
		/// <param name="id"></param>
		public void UpdateWeight(object id)
		{
			// TODO:  ��� FiFoEliminateMethod.UpdateWeight ʵ��
            lock (MapOfWeights)
            { 
                //���Ƴ���������¼���
                MapOfWeights.Remove(id);
                MapOfWeights.Add(id);
            }
		}

		#endregion
	}
}
