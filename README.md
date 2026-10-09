## 개요
개인 프로젝트로 제작한 3D 액션 Morpg "Project : Combo" 입니다.
<br />
<br />
게임 '명조 : 워더링웨이브', '마비노기 영웅전' 에서 모티브를 얻어 제작한 게임으로,<br />
3-2 캡스톤 프로젝트로 제작하였던 3D 액션게임 Project : Origo의 완성형을 목표로 제작하였습니다.

<br />
<br />

<p align="center">
<img src = "https://github.com/user-attachments/assets/0887308f-4dd4-4804-9535-efceb804711f" width="100%" height="100%"/>
<img src="https://github.com/user-attachments/assets/db8d9098-51f2-41b4-b46b-ae32f1f4e944" width="49%" height="49%"/>
<img src="https://github.com/user-attachments/assets/4fbb9a64-2288-49fd-925a-3365a25f75ef" width="49%" height="49%"/>
<img src="https://github.com/user-attachments/assets/64e8ebe6-4414-42ac-b925-2a50acac5ee0" width="100%" height="100%"/>
</p>

## 요약
- 장르 : 3D 액션 게임
- 제작 기간 : 25.03 ~ 25.09 (약 6개월)
- 개발 목표 : 마비노기 영웅전의 스매시 기능 구현, Json을 활용한 데이터 로컬 저장
- 기술 문서 : https://docs.google.com/presentation/d/1TijUUmfYp6Dh7DBAQQQgyUfkHiZ0gl0DBnp1yTRpF9I/edit?usp=sharing
- 플레이 영상 : https://www.youtube.com/watch?v=mEXa7664BfE

<br />

## 개발 환경
- Unity 2022.03.16f1 LTS
- C#
- Window 11
- PhotoShop
<br />

## 주요 구현
## **1. 플레이어**

**A. 구현 정리**

  - WASD 이동, F 상호작용, Shift 대쉬, C 락온
  - 유저 인풋을 받아오는 인풋 시스템 구현
  - 일반공격 4타
  - 스매시 4타 (스매시 1~3타는 각성 상태에서 사용 시 추가 입력으로 추가 공격 동작)
  - 스킬 4종 (돌진연참 - 각성 사용 시 스킬 변경, 카운터, 각성, 각성기)
  - 아머 구현 3종 (없음, 경직 면역, 다운 면역)
  - 미니맵 & 전투 돌입 시 미니맵 사이즈 조절 구현
  - 공격 사용 시 연계 공격 가이드 UI 구현
<br />

**B. 설계 방향**

**1-1 공격 기능**<br />
<br />
플레이어의 공격 시스템은 단일책임원칙(SRP)을 기반으로 부모 클래스에 공용 함수를 두고<br />
호출자는 공격의 세부 내용을 알 필요 없이 함수를 호출하기만 하면<br />
공격의 세부 기능은 호출된 스크립트에서 알아서 동작할 수 있도록 작업하였습니다.<br />

스킬 특성상 공통적으로 필요한 기능(초기화, 동작 호출 함수 등)을 고려하였을 때,<br />
내용 선언이 가능한 abstract가 적합하다 판단하여 abstract class로 Attack_Base 구현하였습니다.<br />

Attack_Base에는 동작을 호출하는 Use() 함수와 동작을 초기화하는 Reset() 함수가 존재하며<br />
Attack_Base를 상속받아 구현된 세부 동작 기능에서 Use()와 Reset() 함수를 구현하는 방향으로 구현하여,<br />

외부에서는 내부 동작을 알 필요 없이 플레이어의 입력을 받아 해당되는 Use() 함수를 호출하기만 하면<br />
내부 동작이 모두 이루어지도록 설계하였습니다.<br />
<br />

**1-2 플레이어 입력**<br />
<br />
과거 Project : Origo의 문제점 중 하나로 판단한 것이 바로 플레이어의 입력 관리입니다.<br />
과거 프로젝트를 진행할때는 각각의 동작 스크립트에서<br />
Input.Getkey...()로 직접 입력을 받아 동작하도록 구현하였습니다.<br />

문제는 스크립트들이 각자 입력을 받아 동작하게 구현할 경우<br />
A 기능이 동작하는 과정에서 B 입력이 들어오면 두 기능이 모두 동작하는 문제가 발생하며,<br />
이를 막기 위해 스크립트들이 서로 필요 이상으로 결합되는 문제가 발생하는 것입니다.<br />
<br />

이를 해결하기 위해 Input_Manager를 제작하여 플레이어의 입력을 처리하는 전용 매니저를 구현하였습니다.<br />
Input_Manager에는 입력이 들어왔을 때,<br />
어떤 동작을 호출할 것인가에 대한 데이터를 담은 InputData 데이터 클래스가 존재합니다.<br />

해당 데이터는 List<InputData> inputDatas;와<br />
List<InputData> shortcutInputDatas; 2가지로 나눠 관리되고 있으며,<br />

    public void Action_Setting(int index, Action name)
    {
        // 혹시 모를 함수 중복 등록 방지용
        if(inputDatas[index].inputAction != null)
        {
            Debug.Log($"중복 등록 감지 {index} 번째 데이터 / {name} 함수 / {inputDatas[index].inputAction} 등록되어 있었음");
            inputDatas[index].inputAction = null;
        }

        inputDatas[index].inputAction += name;
    }

    public void ShortCut_Setting(int index, Action name)
    {
        shortcutInputDatas[index].inputAction += name;
    }
    
각각의 List에 데이터를 세팅하는 함수 Action_Setting(int index, Action name)와 <br />
ShortCut_Setting(int index, Action name)로 List에 동작 데이터를 담습니다.<br />

담겨진 데이터는 Updata() 함수에서 Shortcut_Check(), KeyDown_Check(), KeyUp_Check() 함수를 통해<br />
입력이 있을 경우 InputData 내의 액션을 호출하여 동작합니다.<br />

<br />
<br />


## **2. 장비 & 인벤토리**

**A. 구현 정리**

  - 아이템은 장비, 소비, 기타 3종
  - 아이템 티어는 1~5티어로 구분 (구현은 3티어까지)
  - 장비 종류 6종 구현 (머리, 몸통, 하의, 신발, 무기, 코어)
  - 3티어 이상의 장비부터 고유의 장비효과 추가 구현 (타격 시 회복, 투사체 발사 등)
  - 소모품은 즉시 회복, 지속회복 2종 구현
  - 인벤토리 정리 기능 구현 (정렬 기준은 장비 -> 소모품 -> 기타 순서 + 희귀도 기준 정렬)
  - 소모품 쇼트컷 구현 (최대 4개 / 쿨타임 표기 / 전부 소진 시 자동 비우기)
<br />

**B. 설계 방향**

장비 & 인벤토리 시스템 구현에서 고려한 부분은 데이터의 관리 주체 결정이었습니다.<br />
데이터 보유 책임을 각각의 슬롯에게 일임할지,<br />
매니저가 모든 데이터를 보유하고 슬롯은 UI 표시만 담당할지 선택해야 했습니다.<br />

슬롯끼리 아이템 스왑(교체), 장비 장착과 같은 기능을 고려하였을 때,<br />
슬롯은 자신이 어떤 아이템을 들고 있는지 알 필요가 있었습니다.<br />

결과 아이템 추가 & 제거 로직은 Inventory_Manager가 관리하고<br />
아이템의 종류, 보유 데이터는 Inventory_Slot이 관리하도록 구현하였습니다.<br />
<br />
<br />

## **3. 레벨 업**

**A. 구현 정리**

  - 경험치 및 레벨업 시스템
  - 자동 필요 경험치 계산 기능
  - 레벨업 시 스킬포인트 & 스테이터스 증가 기능 (5레벨 단위로 추가 증가)
<br />


**B. 설계 방향**

레벨업 시스템에서 고려하였던 부분은 레벨업을 위한 필요 경험치 데이터 자동 생성과<br />
특정 단위(본 프로젝트에서는 5레벨)의 추가 스테이터스 증가 로직입니다.<br />

필요 경험치 데이터의 경우 수동으로 작성하여 삽입할 수도 있으나,<br />
레벨 단위가 수백, 수천을 넘어가기 시작하면 한계에 맞닥드릴수 밖에 없다 생각하였습니다.<br />

이를 해결하기 위해 조건식을 통해 자동으로 레벨업에 필요한 경험치 데이터를 생성하자 판단하였고,<br />
거듭제곱(Polynomial) 성장식을 사용하여 요구 경험치 데이터를 생성하였습니다.<br />

데모 프로젝트 특성 상 많은 레벨 데이터를 요구하는 것도,<br />
그만한 플레이 로직이 구현되는 것도 아니기에<br />
최대 레벨을 25로 결정하고 growth 1.5로 경험치 데이터를 작성하였습니다.<br />

    public void Level_Setting()
    {
        // 최종 레벨
        maxLevel = 25;

        // 경험치 요구량은 지수계수 1.5로 계산
        for (int i = 1; i < maxLevel; i++)
        {
            int a = (int)(baseExp * Mathf.Pow(i, growth));
            expList.Add(a);
        }

        // 현제 경험치
        maxExp = expList[curLevel - 1];
    }
<br />
<br />

## **4. 스킬 & 스킬트리**

**A. 구현 정리**
  - 스킬포인트를 소모하여 스킬 강화
  - 스킬 선택 시 설명 텍스트 및 영상 표시
  - 스킬 강화 시 스킬  계수 및 아머, 피격 효과 변경
  - 기능 동작 스크립트와 데이터 & UI 데이터를 각각 분리 구현
<br />


**B. 설계 방향**

스킬과 스킬트리 시스템 연계에서 중요하게 고려한 부분은<br />
스킬 레벨 관리 주체 결정과 스킬 포인트 관리였습니다.<br />

스킬은 각각 최대 레벨이 상이하며, 레벨에 따라 공격 배율이 달라집니다.<br />
스킬트리는 각각의 스킬이 가진 최대 레벨 데이터와 UI 데이터,<br />
레벨업에 필요한 포인트 데이터가 필요합니다.<br />

스킬 레벨 UI 및 레벨업 관리를 Skill_Manager가,<br />
스킬 데이터는 Attack_Base 보유하는 방식으로 나눠 관리하였으며<br />

스킬트리에서 스킬 레벨 업 버튼을 누를 시, Skill_Manager가 이벤트를 받고<br />
Attack_Base에 데이터를 불러와 조건 확인 후 레벨업이 가능하면 증가,<br />
아니라면 불가 조건에 따른 안내 UI를 표시하도록 구현하였습니다.<br />
<br />
<br />


## **5. 옵션**

**A. 구현 정리**

  - 오디오 믹서를 사용한 사운드 조절 시스템
  - 사운드는 마스터, bgm, sfx 3종으로 분리
  - 프레임레이트 조절 기능
  - 수직 동기화 기능
  - 메인화면 리턴 기능
<br />


**B. 설계 방향**

옵션 시스템의 목표는 오디오 믹서를 활용한 사운드의 타입 별 조절,<br />
프레임레이트 및 수직 동기화 기능 구현을 구현하여 기초적인 옵션 시스템을 구현하는 것입니다.<br />

오디오 믹서를 사용해 Master, SFX, BGM 3가지의 타입으로 사운드를 분리하였으며,<br />
Master의 하위에 SFX, BGM을 넣음으로서 개별 조절 및 Master 사운드 조절을 통한<br />
전체 사운드 조절이 가능하게 구현하였습니다.<br />

    public void Setting_Master(float value)
    {
        Master_Volume = Mathf.Log10(value) * 20;
        mixer.SetFloat("Master", Mathf.Log10(isMasterOn ? value : 0.0001f) * 20);
    }
    
    public void OnOff_Master(bool isOn)
    {
        isMasterOn = isOn;
        mixer.SetFloat("Master", Mathf.Log10(isMasterOn ? Master_Volume : 0.0001f) * 20);
    }
<br />

프레임레이트 & 수직 동기화의 경우 유니티에서 지원하는 드롭다운 UI와<br />
Application.targetFrameRate = value; 를 사용하여 원하는 프레임으로 설정 가능하게 구현하였습니다.<br />
<br />

    public void Setting_FPS(int value)
    {
        flameIndex = value;
        Application.targetFrameRate = isVsync ? -1 : frameDic[flameIndex];
    }

    public void Setting_VSync(int value)
    {
        isVsync = value == 1 ? true : false;
        QualitySettings.vSyncCount = value;
    }
<br />
<br />


## **6. 튜토리얼**

**A. 구현 정리**

  - 각 행동 별 튜토리얼 영상 & 텍스트 표시
  - 우 하단 툴팁 UI 표시
<br />

**B. 설계 방향**

튜토리얼 시스템은 한눈에 알아볼 수 있는 직관적인 디자인과,<br />
튜토리얼을 놓쳤더라도 다시 확인할 수 있는 편의성을 고려하였습니다.<br />

이를 위해 튜토리얼 종류를 화면 전체에서 보여주는 팝업 창과<br />
우측 하단에 표시되는 툴팁 창 2종류로 구현하였으며,<br />
전달성을 높히기 위해 중요 텍스트의 색상 변경을 통한 강조 표시 및 영상을 추가하였습니다.<br />
<br />
<br />


## **7. 몬스터**

**A. 구현 정리**

  - Enemy_Base abstract 클래스 상속 기반 구현
  - 판단 -> 추적 -> 공격 선택 -> 공격 -> 딜레이 FSM 적용
  - 그로기 게이지를 전부 소모할 경우 피격이상(경직, 다운) 효과 적용
  - 가중치 렌덤 아이템 & 경험치 드랍
<br />

**B. 설계 방향** 

<img width="1411" height="812" alt="Image" src="https://github.com/user-attachments/assets/55764d6b-e1cd-4993-893c-1d9a7aed52af" />
<br />
<br />

몬스터 구현에 있어 중요하게 고려한 부분은 FSM 기반 동작 AI와<br />
Abstract 클래스 상속을 통한 공통 기능 묶어 불필요한 중복 스크립트 제거,<br />
공격 데이터 분리로 Prefab 할당 및 관리 편의성 증가입니다.<br />

과거에 제작한 "Project : Origo" 는 단일책임원칙(SRP)을 제대로 지키기 못해<br />
몬스터의 상태 데이터와 행동 데이터가 서로 얽혀 코드 복잡도가 증가했던 문제가 발생하였습니다.<br />
<br />
<br />

이번 프로젝트에서는 Attack_Base를 기반으로 공격의 공용 기능을 담은 abstract class를 제작하고,<br />
해당 스크립트 상속받아 스킬의 세부 기능을 구현하여 몬스터 FSM과 공격 세부 동작을 분리하는 방향으로 구현하였습니다.<br />
또한 능력치, UI 데이터를 각각 SO로 분리하여 관리하여,<br />
데이터 분리 및 기능과 데이터를 분리하여 복잡도 증가를 억제하였습니다.<br />

데미지 계산 및 상태 이상(넉백, 다운) 기능을 인터페이스로 구현하여<br /> 
데미지를 받을 수 있는 객체는 IDamageSysteam 인터페이스로 선언,<br />
플레이어나 몬스터가 아닌 제3의 타입도 적용이 가능하게 설계하여 향후 확장성을 고려하였습니다.<br />

<br />
<br />


## **8. 아지트**

**A. 구현 정리**

  - 아지트 내 스테이지 선택 기능 구현
  - 스테이지 진입을 위한 조건 설정 기능 (이전 스테이지 클리어 등)
  - 스테이지 선택 시 스테이지 설명 & 클리어 랭크 & 클리어 타임 표기 기능
  - 아지트 내 세이브 & 로드 시스템 구현
<br />

**B. 설계 방향** 

아지트는 스테이지 선택, 세이브 & 로드를 진행할 수 있는 거점 구역으로,<br />
스테이지 클리어 데이터를 기반으로 스테이지 진입 가능 여부, 클리어 타임에 따른 랭크 표시 구현을 목표로 하였습니다.<br />
<br />

아지트의 스테이지 선택 기능을 구현하기 위해서는<br />
스테이지의 선택 조건, 입장을 위한 선행 스테이지 클리어 여부에 대한 데이터가 필요하였습니다.<br />

이를 위해 런타임에서 스테이지의 입장가능 여부 및 랭크를 관리하는 매니저 Hideout_Manager를 제작,<br />
아지트 씬에 입장할 때 Save & Load 데이터를 관리하는 SaveLoad_Manager에게 데이터를 받아와<br />
런타임 데이터에 저장된 정보를 적용시켰습니다.<br />

단, 이와 같은 방식으로 스테이지 데이터를 적용할 경우<br />
스테이지 클리어 시 클리어 데이터를 아지트에 복귀하기 전 최신화 해줄 필요가 있었습니다.<br />
<br />

```
[System.Serializable]
public class StageData
{
    // 챕터 내에 존재하는 스테이지 데이터
    public bool isClear;
    public Rank clearRank;
    public float clearTime;
    public enum Rank { N, D, C, B, A, S }
}
```
```
[System.Serializable]
public class ChapterData
{
    // 챕터 데이터 - 각 장의 데이터 묶음
    public string chapterName;
    public List<StageData> stageList;
}
```
```
[System.Serializable]
public class ClearData
{
    // 챕터 묶음 - 최종 저장 데이터
    public List<ChapterData> chapterList;
}
```
<br />

이를 해결하기 위한 데이터 클래스 3종과 플레이어를 따라다니는 ChapterData_Manager를 제작한 뒤,<br />
스테이지를 관리하는 Stage_Manager가 스테이지 클리어 시 데이터를 넘겨받아 작성하는 역할을 부여하여<br />
실시간 데이터 최신화가 가능하게 구현하였습니다.<br />

    public void ClearData()
    {
        // 데이터 생성
        StageData clearData = new StageData
        {
            isClear = true,
            clearTime = Time.time - startTime,
            clearRank = StageData.Rank.N,
        };

        // 랭크 셋팅
        for (int i = 0; i < stageData.Data.Count; i++)
        {
            if (clearData.clearTime < stageData.Data[i].time)
            {
                clearData.clearRank = stageData.Data[i].rank;
                break;
            }
        }

        // 데이터 전달
        ChapterData_Manager.instance.Data_Updata(chapter, stage, clearData);
    }

프로젝트에 구현된 챕터는 튜토리얼과 1장 뿐이나, 향후 챕터 및 스테이지 추가를 고려하여<br />
ClearData에서 List<ChapterData> 형태로 데이터를 저장하였습니다.<br />

<br />
<br />


## **9. 스테이지 이벤트**

**A. 구현 정리**

  - 스테이지 이벤트 오브젝트 구현 (이벤트 포인트, 힐링, 체크 포인트)
  - 다이얼로그 시스템 구현
  - 퀘스트 시스템 구현
  - 몬스터 스포너 3종 구현 
    - 일반 : 1회 소환 후 전체 처치까지 대기
    - 러시 : 지정된 횟수만큼 몬스터 소환 후 대기
    - 보스 : 보스 소환 후 조건 만족까지 대기

  **B. 설계 방향** 
  
스테이지 내 이벤트 설계에서 중요하게 생각한 부분은 체크 포인트 구현이었습니다.<br />
다른 이벤트의 경우 단순히 플레이어의 스테이터스 수치 조절, <br />
특정 구역에 도달 시 OnTrigger로 이벤트를 호출하는 것이 전부라 구현 난이도가 매우 낮았습니다.<br />

하지만 체크 포인트의 경우 해당 스테이지에서 어디까지 진행했는지,<br />
진행 상황에서 내가 보유한 능력치가 얼마였는지 등, 플레이어의 스테이터스 상태와 더불어<br />
전투 중 패배한 구역의 전투 이벤트를 초기화가 필요하여 구현 과정에서 고민을 하였습니다.<br />
<br />

<img width="1072" height="322" alt="Image" src="https://github.com/user-attachments/assets/0274c258-444a-4264-afcd-173b818e8c80" />
<br />
<br />

    /// <summary>
    /// 체크포인트 저장 - 호출은 체크포인트 오브젝트에서
    /// </summary>
    public void CheckPoint_Seting(Transform pos)
    {
        // 위치 저장
        spawnPos = pos.position;
        spawnRotation = Player_Manager.instance.transform.rotation;
    }
<br />

저는 이 문제를 해결하기 위해 스테이지를 관리하는 Stage_Manager에<br />
체크 포인트 오브젝트와 상호작용 시 호출되는 함수인 CheckPoint_Seting()와<br />
<br />

    /// <summary>
    /// 사망 시 체크포인트 호출 - 동작은 사망 후 플레이어 매니저 부분에서
    /// </summary>
    public void CheckPoint_Call()
    {
        // 스테이지 리셋
        foreach (Field_Base field in spawn)
        {
            if (!field.isClear) field.Field_Reset();
        }

        // 스테이터스 변경
        Player_Manager.instance.status.curhp = Player_Manager.instance.status.maxHp;
        Player_Manager.instance.status.curStamina = Player_Manager.instance.status.maxStamina;
        Player_Manager.instance.status.curAwakening = 0;

        // UI 리셋
        UI_Manager.instance.Hp();
        UI_Manager.instance.Awakening();

        // 플레이어 위치 이동
        PlayerAction_Manager.instance.Pos_Setting(spawnPos, spawnRotation);
    }
<br />

실제 플레이어 사망 시 호출되는 함수 CheckPoint_Call() 함수로<br />
스테이지 내 전투 이벤트를 foreach문으로 순회하여 클리어 한 곳은 그대로 두고,<br />
아직 클리어하지 못하거나 도전 도중 사망한 스테이지만 리셋하는 방식으로 스테이지 초기화를 구현하였습니다.<br />

// 이후 작성 필요 -> 몬스터 스폰 로직 플로우차트와 관련 구현 방식 설명
<br />
<br />


## **10. 세이브 & 로드**
  - json 기반 로컬 세이브 구현
  - 인벤토리는 아이템 코드 & 수량 저장
  - 장비, 쇼트컷은 아이템 코드 저장
  - 챕터 & 스테이지 데이터는 데이터 클래스로 저장
  - 로드 기능은 각각의 매니저가 SaveLoad_Manager에서 데이터를 받아와서 로드
  - 데이터가 없을 경우 신규 데이터 생성 & 세이브
<br />
<br />
