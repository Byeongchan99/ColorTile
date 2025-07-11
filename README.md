# 프로젝트 소개

Unity 기반으로 개발한 **2D 퍼즐 게임**입니다.

플레이어는 빈 칸을 클릭하여 **규칙에 따라 같은 색상의 타일을 제거**하며, 최대한 많은 타일을 제거하는 것이 목표입니다.

게임은 **두 가지 모드(일반 모드, 무한 모드)**를 제공하여 플레이 스타일에 맞게 플레이하실 수 있습니다.

</br>

# 세부 사항

### 기간

2025.03.24 ~ 2025.07.07

### 주요 업무

게임의 기획, 스프라이트 제작, UI/UX 디자인 및 프로그래밍을 포함한 모든 개발 과정을 직접 담당하였습니다.

### itch.io 사이트

[컬러 타일](https://harrrypoter.itch.io/color-tile)

### 플레이 영상

[컬러 타일 플레이 영상](https://youtu.be/mDeWzz7QrYQ?si=QdkBV4R3XcCu3WIO)

</br>

# 게임 구조

![Image](https://github.com/user-attachments/assets/2057f662-7f68-4694-b61f-fdb109e9b334) ![Image](https://github.com/user-attachments/assets/9c655303-c3b2-46b5-8bde-613432075cb7)

- **일반 모드**: 제한 시간 90초 내에 최대한 많은 타일을 제거하는 것이 목표입니다.
    
    잘못된 칸을 클릭할 경우 **5초의 시간 패널티**가 부여됩니다.
    
- **무한 모드**: 제한 시간 없이 플레이할 수 있으며, 잘못된 클릭에 대한 패널티도 없습니다.
    
    모든 타일을 제거하면 **점수를 유지한 채 새로운 판이 생성됩니다.**

</br>

## 게임 규칙

플레이어는 **빈 칸을 클릭하여 같은 색상의 타일을 제거**할 수 있습니다.

조건은 **상하좌우 방향으로 인접한 선상에 같은 색상의 타일이 존재할 때**이며, 클릭한 칸을 기준으로 **가장 가까운 동일 색상의 타일들**이 자동으로 매칭되어 제거됩니다.

위 이미지에서처럼, 노란색 별이 표시된 지점을 클릭하면 해당 위치에서 상하좌우 방향으로 가장 가까운 같은 색상의 타일들이 탐색되어 동시에 제거됩니다.

</br>

### 초기 기획 및 설계

[컬러 타일 기획 및 설계](https://www.notion.so/1c04e320564d8091bb59c837480678a4?pvs=24)

<br/>

# 주요 기능

</br>

1. **스테이지 생성**

![Image](https://github.com/user-attachments/assets/e578afae-b851-419f-b053-904d4697b90d)

***스테이지 생성 과정을 시각화하기 위해 연출한 이미지입니다.***

</br>

[컬러 타일 개발 일지 1 - 스테이지 생성 로직](https://www.notion.so/1-1e34e320564d80de8335efd6f1760e23?pvs=21) 

</br>

클리어 불가능한 경우를 방지하기 위해, 항상 해결 가능한 구조로 스테이지가 생성되도록 로직을 구성하였습니다.

</br>

2. **UI 해상도 관리**

</br>

<img width="300" height="600" alt="Image" src="https://github.com/user-attachments/assets/4a1d1694-d7f4-42a3-9776-17cc8e7604b8" />

<img width="300" height="600" alt="Image" src="https://github.com/user-attachments/assets/a597de57-26b6-4b95-b983-1b0fe158e044" />

<img width="300" height="600" alt="Image" src="https://github.com/user-attachments/assets/de5130b1-f99e-4bf6-8153-cb987e22b169" />

</br>
</br>

[컬러 타일 개발 일지 2 - 임시 UI 및 해상도 작업](https://www.notion.so/2-UI-1f04e320564d807ea972c236b81fbb0d?pvs=21) 

[컬러 타일 개발 일지 4 - UI 작업 마무리 및 색약 모드](https://www.notion.so/4-UI-2144e320564d80018a3fef213ac7fbd4?pvs=21) 

</br>

다양한 모바일 해상도에 대응할 수 있도록, UI 요소들이 자동으로 정렬되고 크기가 조정되도록 하였습니다.

</br>

3. **시각 효과**

![Image](https://github.com/user-attachments/assets/45112136-84b5-454f-bdfd-74b5554727d5)   ![Image](https://github.com/user-attachments/assets/6ddabe92-b13f-4e3d-9051-65e3f9da4c53)

</br>

[컬러 타일 개발 일지 3 - 디테일 추가](https://www.notion.so/3-1f04e320564d801bba09ed9ac9a0b3d2?pvs=21) 

</br>

시각적 피드백과 몰입도를 높일 수 있는 오버레이 효과, 타일 제거 애니메이션, 화면 전환 애니메이션과 같은 여러 시각 효과를 추가하였습니다.

</br>

# 프로젝트 경험

[컬러 타일 개발 개선점 및 아쉬웠던 점](https://www.notion.so/1f04e320564d80d68d09d7381b84f341?pvs=21) 

</br>

- Action 사용해서 최대한 디커플링 해보기
- ‘반드시 클리어할 수 있는’ 스테이지 생성 알고리즘 적용하기
- 다양한 디바이스에 대한 해상도 맞추기
    - 다양한 해상도에 맞게 UI와 게임 오브젝트들 자동으로 정렬하기
    - 다양한 해상도에 맞게 UI와 게임 오브젝트들 자동으로 크기 변경하기
- 다양한 효과 적용하기
    - 오버레이 효과 적용하기
    - 타일 제거 애니메이션 적용하기
    - 화면 전환 애니메이션 적용하기
    - 색약 모드 적용하기
    - 진동 세기 조절하여 적용하기

</br>

## 발생했던 문제 및 해결 방법

UI 개발 중 발생한 문제점으로, 디스플레이 크기에 따라 Mid Area는 자동으로 크기가 변경되지만, 게임 오브젝트인 보드판은 크기가 자동으로 변경되지 않아 레이아웃에 문제가 발생했습니다.

<img width="500" height="317" alt="Image" src="https://github.com/user-attachments/assets/b2589876-8e01-4ec8-814d-ea1fe943ef2a" /> <img width="500" height="317" alt="Image" src="https://github.com/user-attachments/assets/ffab6af6-c513-4c04-8ae2-f8364b5235e6" />

***검은색 박스: Mid Area, 빨간색 박스: 보드판***

</br>

구체적으로, Mid Area가 디스플레이 크기에 맞춰 커지면 보드판과 Mid Area 사이의 간격(padding)이 비정상적으로 커지거나, 반대로 Mid Area가 작아지면 보드판이 잘려서 보이는 문제가 있었습니다.

이를 해결하기 위해 보드판을 UI 오브젝트로 변환하고, Mid Area의 자식 UI 오브젝트로 배치하여 관리했습니다. 

확장 및 border 속성을 활용하여 보드판의 크기를 조정하였고, 이로 인해 보드판의 정확한 크기를 구하기 쉬워졌습니다. 또한, 보드판 크기를 기준으로 타일 크기를 계산하고 변수들을 설정할 수 있게 되어 전체 레이아웃 관리가 더욱 용이해졌습니다.
