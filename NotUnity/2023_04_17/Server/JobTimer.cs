using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
	struct JobTimerElem : IComparable<JobTimerElem>
	{
		// 실행 시간
		public int execTick;
		// 해야할 행동
		public Action action;

		// 실행시간 비교
		public int CompareTo(JobTimerElem other)
		{
			// 상대방 틱 - 나의 틱
			return other.execTick - execTick;
		}
	}

	class JobTimer
	{
		PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();
		object _lock = new object();

		public static JobTimer Instance { get; } = new JobTimer();
		
		// tickAfter = 몇틱 후 실행을 할 것이냐
		public void Push(Action action, int tickAfter = 0)
		{
			JobTimerElem job;
			// 언제 실행하는지 계산
			job.execTick = System.Environment.TickCount + tickAfter;
			// 실행해야할 액션
			job.action = action;

			lock (_lock)
			{
				// 원자적으로 해야할 job 입력
				_pq.Push(job);
			}
		}

		public void Flush()
		{
			while (true)
			{
				int now = System.Environment.TickCount;

				JobTimerElem job;

				lock (_lock)
				{
					if (_pq.Count == 0)
						break;

					// 다음 실행될 최상위 큐 내용 조회
					job = _pq.Peek();
					// 실행해야할 시간이 아직이라면, 탈출
					if (job.execTick > now)
						break;

					// 실행해야할 시간이라면 팝 => 실행
					_pq.Pop();
				}

				// 실행
				job.action.Invoke();
			}
		}
	}
}
