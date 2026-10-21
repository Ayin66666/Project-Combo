
## 개요

<p align="center">
<img src = "https://github.com/user-attachments/assets/0887308f-4dd4-4804-9535-efceb804711f" width="100%" height="100%"/>
</p>

개인 프로젝트로 제작한 3D 액션 게임 'Project : Combo' 입니다.<br />

게임 '명조 : 워더링웨이브', '마비노기 영웅전' 에서 모티브를 얻어 제작한 프로젝트로,<br />
3-2 대학 캡스톤 수업에서 팀 프로젝트로 제작하였던 3D 액션 게임<br />
'Project : Origo' 의 아쉬운 점을 보완한 완성형을 목표로 제작하였습니다.
<br />

## 요약
- 게임 장르 : 3D 액션 게임
- 제작 기간 : 25.03 ~ 25.09 (약 6개월)
- 개발 목표 : 게임 '마비노기 영웅전'의 일반공격 & 스매시 전투 시스템 구현 및 Json을 활용한 데이터 로컬 저장
- 기술 문서 : https://docs.google.com/presentation/d/1TijUUmfYp6Dh7DBAQQQgyUfkHiZ0gl0DBnp1yTRpF9I/edit?usp=sharing
- 동작 영상 : https://www.youtube.com/watch?v=mEXa7664BfE
<br />

## 개발 환경
- Unity 2022.03.16f1 LTS
- C#
- Windows 11
- PhotoShop 2019
<br />

## 주요 구현
## **1. 플레이어**
<img src="https://github.com/user-attachments/assets/64e8ebe6-4414-42ac-b925-2a50acac5ee0"/><br />

### **A. 구현 정리**

#### ▣ 플레이어 조작
  - WASD 이동, F 상호작용, Shift 대쉬, C 락온
  - 유저 입력 처리 시스템 구현

#### ▣ 전투 & 공격
  - 일반공격 4타 및 스매시 4타 (스매시 1~3타는 각성 상태에서 사용 시 추가 입력으로 추가 공격 동작)
  - 스킬 4종 및 스킬 '각성' 사용 시 공격에 추가 조작 활성화
  - 아머 구현 3종 (없음, 경직 면역, 다운 면역)

#### ▣ UI & 성장
  - 미니맵 & 전투 돌입 시 미니맵 사이즈 조절 구현
  - 공격 사용 시 연계 공격 가이드 UI 구현
  - 경험치 누적에 따른 레벨업 처리 및 레벨별 필요 경험치 계산
<br />

플레이어 기능 구현에서 중요하게 고려한 부분은 기능과 유저 입력의 분리 관리,<br />
추상 클래스를 기반 플레이어 공격 시스템의 설계,<br />
단일책임원칙(SRP) 기반 스크립트 분리 및 분리된 스크립트의 관리 구조 개선입니다.<br />
<br />

---

### **B. 사례**

#### **B-1. 입력 처리와 행동 실행 구조 설계**

- **문제점**<br />
기능 & 입력의 혼재로 인한 스크립트 강결합 문제 발생
  
- **고민**<br />
과거에는 프로젝트를 진행할 때 각각의 동작 스크립트가 Input.Getkey...()로 직접 입력을 처리하였습니다.<br />
이 경우 여러 동작에 입력이 동시에 전달되어 의도한 동작이 이루어지지 않을 수 있으며,<br />
이를 막기 위해 스크립트 간 의존성이 증가하는 문제가 발생하였습니다.<br />
  
- **해결책**<br />
플레이어의 입력을 처리하는 전용 매니저 Input_Manager를 제작하여 입력 처리를 분리하고,<br />
PlayerAction_Manager 현재 행동 가능 여부를 확인한 뒤<br />
동작 로직이 담긴 Attack_Base를 실행하도록 설계하였습니다.<br />
<br />

Input_Manager는 입력마다 호출할 동작을 데이터 클래스 InputData에 Action으로 등록하고,<br />
매니저에서 입력을 받아 입력마다 할당된 동작을 호출합니다.<br />

```
    [Serializable]
    public class InputData
    {
        public string keyName;
        public KeyCode key;
        public bool isInput;

        public Action inputAction;
    }
```

입력 데이터는 일반 입력과 쇼트컷 입력을 구분하여<br />
각각 List<InputData> inputDatas와 List<InputData> shortcutInputDatas에서 관리합니다.<br />
```
    public void Action_Setting(int index, Action name)
    {
        if(inputDatas[index].inputAction != null)
        {
            Debug.Log($"중복 등록 감지 {index} 번째 데이터 / {name} 함수 / {inputDatas[index].inputAction} 등록되어 있었음");
            inputDatas[index].inputAction = null;
        }

        inputDatas[index].inputAction += name;
    }
```
각 입력에 대응하는 동작은 Action_Setting(int index, Action name)와 <br />
ShortCut_Setting(int index, Action name)로 List에 동작 데이터를 담습니다.<br />

데이터는 Update()에서 Shortcut_Check(), KeyDown_Check(), KeyUp_Check()를 통해 입력을 체크하며<br />
입력이 있을 경우 InputData에서 할당된 액션을 호출하여 동작합니다.<br />

```
    private void Attack_Normal()
    {
        if (!canAction || !canAttack || isAttack || isSmash)
        {
            return;
        }
        else
        {
            // 최대 공격 횟수 체크
            if (attackCount < 4)
            {
                foreach (Attack_Base attack in normalAttacks)
                {
                    attack.Attack_Reset();
                }

                isAttack = true;
                attackCount++;
                anim.SetInteger("AttackCount", attackCount);

                normalAttacks[attackCount - 1].Use();
            }
        }
    }
```
<br />

Input_Manager의 동작 입력은 PlayerAction_Manager의 KeyActionSetting() 함수에서 Attack_동작이름()을 등록하며,<br />
각 행동 함수에서는 isAttack, isSmash 등 상태를 검사하여 실행 가능 여부를 결정합니다.<br />

실행이 가능하면 Attack_Base를 기반으로 관리하는 개별 공격 스크립트의 Use()함수를 호출하여 동작을 실행합니다.<br />
<br />

---

#### **B-2. 스크립트 분리 및 관리**

- **문제점**<br />
  여러 책임이 일부 매니저에 집중되어 코드 수정 범위와 유지보수 부담 증가<br />
  단일책임원칙(SRP) 기반 스크립트 분리에 따른 매니저 증가 문제<br />

- **고민**<br />
  과거 프로젝트에서는 Player_Manager와 UI_Manager만 두고<br />
  내부에 대부분의 플레이어 & 플레이어 UI 관련 로직을 넣어 시스템을 구성하였습니다.<br />

  하지만 이같은 방식은 단일책임원칙(SRP)가 제대로 지켜지지 않아 버그 발생 시 문제점을 찾기 힘들며<br />
  팀원과 협업 과정에서도 양쪽 다 해당 스크립트를 수정해야 하는 상황이 나왔을 때,<br />
  수정 후 머지 과정이 복잡해지는 문제가 발생하였습니다.<br />

  이 문제를 해결하기 위해 단일책임원칙(SRP)를 기반으로 각 기능을 나눠 구현하였습니다.<br />
  그러나 책임별로 스크립트를 분리하는 과정에서 플레이어를 관리하는 매니저의 수가 늘어나<br />
  진입점이 대폭 늘어나게 되었습니다.<br />

- **해결책**<br />
  모든 매니저를 싱글톤으로 구현할 경우 매니저마다 독립적인 진입점이 생겨<br />
  싱글톤 남용으로 관리 구조가 복잡해질 수 있다고 판단하였습니다.<br />
  이를 해결하기 위해 플레이어 관련 매니저의 관리 허브 Player_Manager를 구현하여 진입점을 통일하였습니다.<br />
  <br />

<img width="1179" height="686" alt="Image" src="https://github.com/user-attachments/assets/0077298d-85ff-47bc-830e-5abfa3a5f8d3" /><br />

Player_Manager를 통해 각 매니저에 접근할 수 있도록 구성함으로써<br />
기능별 스크립트 분리 구조를 유지하면서도 분리된 매니저의 접근 및 관리 경로를 일관되게 유지하고,<br />
협업 과정에서 매니저의 수정이 필요할 때 작업 영역이 최소한으로 겹치도록 설계하였습니다.<br />
<br />
<br />


## **2. 몬스터**

### **A. 구현 정리**

▣ 설계 & 동작 로직
  - Enemy_Base 추상 클래스 상속 기반 구현
  - Attack_Base 추상 클래스 기반 공격 로직 작성
  - FSM 적용 (판단 -> 추적 -> 공격 선택 -> 공격 -> 딜레이)

▣ 구현 기능
  - 그로기 게이지를 전부 소모할 경우 피격이상(경직, 다운) 효과 적용
  - 가중치 랜덤 기반 아이템 드랍
  - 처치 시 경험치 드랍
<br />

몬스터 구현에서 중요하게 고려한 부분은 FSM 기반 동작과<br />
추상 클래스를 활용하여 공통 기능을 묶고 불필요한 중복 스크립트 제거,<br />
판단, 공격 스크립트 분리로 공격 별 Prefab 할당 및 관리 편의성 증가입니다.<br />

---

### **B. 사례** 

#### **B-1. 판단, 동작 로직 분리**

- **문제점**<br />
  몬스터의 판단, 동작 로직이 하나의 스크립트에 모두 작성되어 코드 복잡도 증가<br />
  프리팹, 오브젝트 할당 및 수정 작업의 복잡도 증대<br />

- **고민**<br />
  과거 제작한 'Project : Origo' 는 단일책임원칙(SRP)을 제대로 지키기 못해<br />
  몬스터의 판단 로직과 동작 로직이 하나의 스크립트에 모두 작성되어 있었습니다.
  이로 인해 코드 복잡도 상승, 프리팹 및 오브젝트 할당 및 수정 난이도 증가 문제가 발생하였습니다.<br />

- **해결책**<br />
  이번 프로젝트에서는 Attack_Base를 기반으로 공격의 공용 기능을 담은 abstract class를 제작하고,<br />
  해당 스크립트 상속받아 스킬의 세부 기능을 구현하여 몬스터의 판단 로직과 공격 세부 동작을 분리하여 설계하였습니다.<br />
  또한 능력치, UI 데이터 역시 각각 SO로 분리하여 관리하여 단일 스크립트의 책임 과중을 경계하였습니다.<br />
  <br />

  <img width="1411" height="812" alt="Image" src="https://github.com/user-attachments/assets/55764d6b-e1cd-4993-893c-1d9a7aed52af" /><br />

---

#### **B-2. 인터페이스 기반 데미지 로직 분리**

- **문제점**<br />
  데미지 계산 로직이 Player, Enemy에 독립적으로 구현되어 있어 접근이 어려움
  제3의 피격 가능 객체 구현 시 Player, Enemy에 포함된 불필요 기능이 강제됨
  
  
- **고민**<br />
  과거 제작한 'Project : Origo'는 프로젝트는 플레이어, 몬스터에게 각각 TakeDamage() 함수를 구현하고,<br />
  공격 범위 내에서 플레이어 & 몬스터 검출 시 Tag로 플레이어 or 몬스터를 식별한 뒤<br />
  Getcomponent().TakeDamage()로 데미지를 전달하였습니다.<br />

  문제는 이와 같은 구조로 데미지 시스템을 구현하면<br />
  데미지를 받을 수 있는 객체는 무조건 플레이어, 혹은 몬스터이어야 합니다.<br />
  
  플레이어나 몬스터가 아닌 제3의 데미지를 받을 수 있는 객체에 대한 확장성이 전무하며<br />
  Enemy_Base를 상속받아 구현하더라도 데미지 기능만 필요한 객체가 불필요한 기능을 포함하게 됩니다.<br />
  
- **해결책**<br />
  데미지 계산 및 상태 이상(넉백, 다운) 기능을 인터페이스로 구현하여<br /> 
  데미지를 받을 수 있는 객체는 IDamageSystem 인터페이스로 선언,<br />
  플레이어나 몬스터가 아닌 제3의 타입도 적용이 가능하게 설계하여 향후 확장성을 고려하였습니다.<br />
<br />
<br />


## **3. 세이브 & 로드**

### **A. 구현 정리**

  - json 기반 로컬 세이브 구현
  - 인벤토리는 아이템 코드 & 수량 저장
  - 장비, 쇼트컷은 아이템 코드 저장
  - 챕터 & 스테이지 데이터는 데이터 클래스로 저장
  - 로드 기능은 각각의 매니저가 SaveLoad_Manager에서 데이터를 받아와서 로드
  - 데이터가 없을 경우 신규 데이터 생성 & 세이브
<br />

### **B. 설계 방향** 

지금까지 대학교에서 제작한 개인 & 팀 프로젝트는 모두 외부 전시 및 시연에 초점이 맞춰진 프로젝트로,<br />
전시 및 시연의 특성 상 데이터를 저장할 필요가 없었기 때문에 관련 문제를 고려할 필요가 없었습니다.<br />

하지만 단순 시연 목적이 아니라면 아무리 간단한 게임이라도<br />
진행사항이나 점수 등 데이터 세이브 & 로드 기능이 있는 경우가 대부분입니다.<br />

<img width="792" height="302" alt="Image" src="https://github.com/user-attachments/assets/84cefd91-25b1-44fa-9ea1-d238f486116c" /><br />
<br />

처음 세이브 & 로드 기능을 구현하는 프로젝트인 만큼 온라인 저장 기능을 구현을 도전하기보다는<br />
사용자의 컴퓨터 저장공간에 데이터를 저장하는 단계부터 천천히 밟아가는 것이 좋겠다 판단하여,<br />
Unity에서 지원하는 JsonUtility를 기반으로 로컬 세이브 기능을 구현하기로 결정하였습니다.<br />
<br />

```
try
{
    string json = JsonUtility.ToJson(data);
    File.WriteAllText(savePath + fileName[slotCount], json);

    // 데이터 적용 - 스테이터스
    Player_Manager.instance.status.Status_Setting(data);

    // 데이터 적용 - 경험치
    Player_Manager.instance.status.Level_Setting();

    // 데이터 적용 - 챕터
    ChapterData_Manager.instance.Data_Setting(data);

    // 데이터 적용 - 스킬트리
    Player_Manager.instance.skill.Skill_Setting(data);

    return (data, true);
}
catch (Exception e)
{
    Debug.LogError($"저장 에러 발생! {e.Message}");
    return (null, false);
}

```
프로젝트 목표가 4개의 세이브 슬롯을 지원하는 만큼, 각 세이브에 접근하기 위한 명확한 경로 작성이 필요하였습니다.<br />
이를 해결하기 위해 기본 파일 경로를 string 로 저장하고, 파일 이름을 save1~4로 저장하여,<br />
각 슬롯을 클릭할 경우, 경로 + 파일 이름으로 필요한 파일의 접근 경로를 제작하는 방식으로 구현하였습니다.<br />

```
[System.Serializable]
public class Data
{
    [Header("---Ohter---")]
    public string chapter;
    public float playTime;
    public string SceneName;
    public Vector3 playerPos;

    [Header("---Status---")]
    public int curLevel;
    public int curExp;

    // Defence Status
    public int curhp;
    public int maxHp;
    public int physicalDefence;
    public int magicalDefence;

    // Attack Status
    public int physicalDamage;
    public int magicalDamage;
    public float attackSpeed;
    public float criticalhit;
    public float critical_multiplier;

    // Other Status
    public float moveSpeed;
    public float curStamina;
    public float maxStamina;
    public float curAwakening;
    public float maxAwakening;
    public float staminaRecovery;

    [Header("---Item---")]
    public List<ItemData> itemData;
    public List<int> equipment;
    public List<int> shortcut;

    [Header("---Chapter---")]
    public ClearData claerData;

    [Header("---Skill Tree---")]
    public int skillPoint;
    public List<int> skillLevelData;
}
```
Json으로 변환되어 저장하는 데이터는 Class Data로<br />
플레이어의 능력치, 인벤토리, 장비, 스킬, 클리어 데이터를 가지고 있습니다.<br />

```
[System.Serializable]
public class ItemData
{
    public int itemCode;
    public int itemCount;
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
public class StageData
{
    // 챕터 내에 존재하는 스테이지 데이터
    public bool isClear;
    public Rank clearRank;
    public float clearTime;
    public enum Rank { N, D, C, B, A, S }
}
```

이때 각 파트(인벤토리, 클리어 데이터 등)은 개별 데이터 클래스를 구현하여 저장하였습니다.<br />
하나의 파일을 저장함에도 개별 클래스로 분리한 이유는 SaveLoad_Manager가<br />
필요 이상으로 다른 매니저와 결합되는 것을 경계하기 위해서였습니다.<br />

저장되는 데이터는 각자 매니저를 보유한 기능들로,<br />
런타임 데이터 역시 해당 매니저들이 보유하고 있습니다.<br />

이때 저장되는 데이터와 런타임 데이터의 구성이 다르므로<br />
저장을 위해 런타임 데이터를 세이브용 데이터로 전환하는 과정이 필요했습니다.<br />
<br />


저는 이 과정에 대한 책임을 어느 스크립트에 줄지 고민하였고,<br />
결과 SaveLoad_Manager에 런타임 -> 세이브 데이터 전환 책임까지 주는 것은<br />
SaveLoad_Manager의 책임 영역 밖의 일이라 판단하였습니다.<br />

SaveLoad_Manager가 런타임 데이터의 전환까지 책임지게 구현했을 경우<br />
런타임 데이터가 변경되면 거기에 맞춰 전환 로직도 교체해야 하는데,<br />
데이터 스크립트와 전환 로직이 분리되어 있다면 교체 지점이 늘어나 작업 난이도가 상승하게 됩니다.<br />

또한 전환 작업을 위해 이를 위해 SaveLoad_Manager가<br />
필요 이상으로 다른 스크립트와 결합하게 될 것으로 판단하였습니다.<br />

```
// SaveLoad_Manager -> Save(int index) 일부 발췌
{
    // 스킬트리
    skillPoint = Player_Manager.instance.skill.skillPoint,
    skillLevelData = Player_Manager.instance.skill.GetSkillData(),

    // 아이템
    itemData = new List<ItemData>(),
    equipment = Player_Manager.instance.equipment.GetEquipmentData(),
    shortcut = Player_Manager.instance.shortCut.GetShortcutData(),

    // 진행도
    chapter = ChapterCheck(index),
    playTime = PlayTime(),
    SceneName = SceneManager.GetActiveScene().name,
    playerPos = Player_Manager.instance.action.transform.position,

    // 스테이지 클리어
    claerData = ChapterData_Manager.instance.claerData.claerData,
}
```

최종적으로 SaveLoad_Manager는 다른 매니저의 전환 및 데이터 전달 함수 Get...()을 호출하면<br />
요청받은 매니저가 런타임 데이터에서 세이브용 데이터로 전환한 뒤 SaveLoad_Manager에게 전달하고,<br />
해당 데이터를 기반으로 Data class를 작성하여 사용자의 컴퓨터에 저장되는 방식으로 구현하였습니다.<br />
<br />
<br />



## **4. 장비 & 인벤토리**

<img src="https://github.com/user-attachments/assets/4fbb9a64-2288-49fd-925a-3365a25f75ef"/><br />

### **A. 구현 정리**

#### ▣ 아이템
  - 아이템은 장비, 소비, 기타 3종
  - 아이템 티어는 1~5티어로 구분 (구현은 3티어까지)
  - 장비 종류 6종 구현 (머리, 몸통, 하의, 신발, 무기, 코어)
  - 3티어 이상의 장비부터 고유의 장비효과 추가 구현 (타격 시 회복, 투사체 발사 등)
  - 소모품은 즉시 회복, 지속회복 2종 구현

#### ▣ 인벤토리 & 쇼트 컷
  - 인벤토리 정리 기능 구현 (정렬 기준은 장비 -> 소모품 -> 기타 순서 + 희귀도 기준 정렬)
  - 소모품 쇼트 컷 구현 (최대 4종 등록 가능 / 사용 쿨타임 표기 / 소진 시 쇼트 컷 자동 비우기)

#### ▣ 장비창
  - 장비 장착 & 해제 기능 구현
  - 장착 & 해제에 따른 플레이어 능력치 증감 표시
<br />

### **B. 설계 방향**

장비 & 인벤토리 시스템 구현에서 고려한 부분은 데이터의 관리 주체 결정이었습니다.<br />
데이터 보유 책임을 각각의 슬롯에게 일임할지,<br />
매니저가 모든 데이터를 보유하고 슬롯은 UI 표시만 담당할지 선택해야 했습니다.<br />

슬롯끼리 아이템 스왑(교체), 장비 장착과 같은 기능을 고려하였을 때,<br />
슬롯은 자신이 어떤 아이템을 들고 있는지 알 필요가 있었습니다.<br />

결과 아이템 추가 & 제거 로직은 Inventory_Manager가 관리하고<br />
아이템의 종류, 보유 데이터는 Inventory_Slot이 관리하도록 구현하였습니다.<br />
<br />
<br />


## **5. 스킬 & 스킬트리**

### **A. 구현 정리**
  - 스킬포인트를 소모하여 스킬 강화
  - 스킬 선택 시 설명 텍스트 및 영상 표시
  - 스킬 강화 시 스킬  계수 및 아머, 피격 효과 변경
  - 기능 동작 스크립트와 데이터 & UI 데이터를 각각 분리 구현
<br />


### **B. 설계 방향**

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


## **6. 튜토리얼 & 유저 가이드**
<img src="https://github.com/user-attachments/assets/db8d9098-51f2-41b4-b46b-ae32f1f4e944"/><br />
<br />

### **A. 구현 정리**

  - 각 행동 별 튜토리얼 영상 & 텍스트 표시
  - 우 하단 툴팁 UI 표시
<br />

### **B. 설계 방향**

**1-1 튜토리얼**<br />

튜토리얼 시스템은 한눈에 알아볼 수 있는 직관적인 디자인과,<br />
튜토리얼을 놓쳤더라도 다시 확인할 수 있는 편의성을 고려하였습니다.<br />

이를 위해 튜토리얼 종류를 화면 전체에서 보여주는 팝업 창과<br />
우측 하단에 표시되는 툴팁 창 2종류로 구현하였으며,<br />

단순 텍스트 및 이미지만 활용하면 정보 전달력이 부족하다 판단하여<br />
전달성을 높이기 위해 중요 텍스트의 색상 변경을 통한 강조 표시 및 영상을 추가하였습니다.<br />
<br />


**1-2 유저 가이드**<br />

제가 외부 전시회 및 학교 홍보를 다니며 느낀 점은<br />
개발자는 자신이 개발하는 과정에서 수십, 수백번의 테스트를 통해 모두 아는 상태로 플레이하지만,<br />

실제 전시 & 홍보를 통해 체험하는 유저의 경우, 처음 접하는 게임을 플레이하다 보니<br />
게임이 낯설어 튜토리얼을 통해 조작법을 안내하더라도 전부 습득하지 못하는 경우가 발생하였습니다.<br />

오프라인에서 진행되는 전시회라면 직접 안내할 수 있으나<br />
이는 게임 내에서 해결하는 것이 아니기에 궁여지책이라 생각하였습니다.<br />
<br />


본 프로젝트는 게임 '마비노기 영웅전'을 플레이해본 사람이라면 바로 적응이 가능하나<br />
일반 공격 - 스매쉬 공격 시스템을 경험해보지 않았거나 익숙하지 않은 유저라면<br />
스매쉬 사용 및 각성 스킬 사용 시 스매쉬 추가 연계와 같은 조작에 어려움을 겪을 것이라 판단하였습니다.<br />

저는 조작 미숙으로 인한 플레이 경험 저하 문제를 해결하기 위해<br />
게임 'Monster Hunter Wild'의 가이드 시스템을 차용하기 결정하였습니다.<br />

공격 가이드는 플레이어가 일반, 스매쉬, 스킬 등<br />
유저가 공격을 하면 우 상단에 UI로 다음에 연계 가능한 공격을 표시합니다.<br />
이를 통해 연계 가능한 공격을 직관적으로 표시하여 조작 난이도를 낮췄습니다.<br />

<img width="972" height="422" alt="Image" src="https://github.com/user-attachments/assets/b1869eba-491c-49f1-bd8f-b93eb2637d14" />
<br />

```
    public void AttackGuide(string[] nextAttacks)
    {
        if (attackGuideCoroutine != null)
            StopCoroutine(attackGuideCoroutine);
        attackGuideCanvasGroup.DOKill();

        attackGuideCoroutine = StartCoroutine(AttackGuideCall(nextAttacks));
    }
```
```
    private IEnumerator AttackGuideCall(string[] nextAttacks)
    {
        // 텍스트 초기화
        attackGuideCanvasGroup.alpha = 1;

        // 텍스트 활성화 및 설정
        guide_nextAttackText.gameObject.SetActive(true);
        guide_nextAttackText.text = string.Join("\n", nextAttacks);

        // 페이드 딜레이
        yield return new WaitForSeconds(0.5f);

        // 페이드 아웃 (1.25초 동안 서서히 사라짐)
        yield return attackGuideCanvasGroup.DOFade(0, 1.25f).WaitForCompletion();
    }
```

연계 가능한 공격의 데이터는 Skill_Base의 string[] nextAttackData에 작성되어 있으며,<br />
Attack_Base를 상속받아 구현된 공격 스크립트에서 Use() 함수 호출 시<br /> 
UI_Manager.instance.AttackGuide(nextAttackData); 형태로 호출하여 UI에 표시합니다.<br />
<br />
<br />


## **7. 옵션**

### **A. 구현 정리**

  - 오디오 믹서를 사용한 사운드 조절 시스템
  - 사운드는 마스터, bgm, sfx 3종으로 분리
  - 프레임레이트 조절 기능
  - 수직 동기화 기능
  - 메인화면 리턴 기능
<br />


### **B. 설계 방향**

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



## **8. 아지트**

### **A. 구현 정리**

  - 아지트 내 스테이지 선택 기능 구현
  - 스테이지 진입을 위한 조건 설정 기능 (이전 스테이지 클리어 등)
  - 스테이지 선택 시 스테이지 설명 & 클리어 랭크 & 클리어 타임 표기 기능
  - 아지트 내 세이브 & 로드 시스템 구현
<br />

### **B. 설계 방향** 

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

### **A. 구현 정리**

  - 스테이지 이벤트 오브젝트 구현 (이벤트 포인트, 힐링, 체크 포인트)
  - 다이얼로그 시스템 구현
  - 퀘스트 시스템 구현
  - 몬스터 스포너 3종 구현 
    - 일반 : 1회 소환 후 전체 처치까지 대기
    - 러시 : 지정된 횟수만큼 몬스터 소환 후 대기
    - 보스 : 보스 소환 후 조건 만족까지 대기
<br />

### **B. 설계 방향** 
  
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

실제 플레이어 사망 시 호출되는 함수 CheckPoint_Call()에서<br />
스테이지 내 전투 이벤트를 foreach문으로 순회하여 클리어 한 곳은 그대로 두고,<br />
아직 클리어하지 못하거나 도전 도중 사망한 전투 이벤트만 리셋하는 방식으로 스테이지 초기화를 구현하였습니다.<br />
<br />
<br />

<img width="737" height="367" alt="Image" src="https://github.com/user-attachments/assets/8ec7a61a-ee22-4541-a112-ce3ad865a469" />
<br />
<br />

몬스터 소환 로직은 모두 Spawn_Base abstract 클래스를 상속받아 구현되었으며<br />
Spawn_Base에는 모든 스폰 로직이 공통적으로 가지는 스폰 타입,<br />
소환할 몬스터의 종류와 위치가 저장되어 있는 데이터 클래스 EnemySpawnData,<br />
EnemySpawnData를 List로 가지고 있는 SpawnData를 지니고 있습니다.<br />

소환 로직은 SpawnData를 읽어 어떤 라운드에 무슨 몬스터를 소환할지 결정합니다.<br />
몬스터 스폰 로직은 연출에 따라 지정된 몬스터 List로 1회 소환하는 Normal,<br />
List에 지정된 라운드 만큼 소환을 반복하는 Rush,<br />
체력 소모에 따른 특수 연출을 포함한 보스 몬스터 소환에 사용되는 Boss 3종이 있습니다.<br />
<br />
<br />

이때 하나의 스크립트에서 함수로 스폰 타입을 분기하는 대신 Spawn_Base를 abstract Class로 만들고,<br />
해당 클래스를 상속받아 각각의 소환 기능을 따로 구현한 이유는 다음과 같습니다.<br />

첫 번째 이유는 스크립트가 너무 비대해지는 것을 경계하기 위해서입니다.<br />
지금 추가되는 타입은 3종류지만 확장성을 고려했을 때,<br />
단일 스크립트에 여러가지 타입의 동작 기능을 넣는 것은 하나의 스크립트가 너무 비대해진다 판단하였습니다.<br />

두 번째로 각 필드마다 리셋 동작이 다르게 이루어지는데 이를 단일 스크립트로 제어하게 될 경우,<br />
소환 타입의 개수만큼 if문 혹은 switch문을 활용하여 분기하여 체크하게 되고,<br />
이는 코드 가독성을 크게 떨어뜨릴 수 있다 판단하였습니다.<br />
<br />
<br />
