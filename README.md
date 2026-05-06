# IMP

Unity 기반 실습 프로젝트입니다. Unity 6 환경에서 기본 3D 씬과 패키지 구조를 구성하고, 이후 게임/인터랙션 기능을 확장하기 위한 기반 저장소입니다.

## 프로젝트 개요

`IMP`는 Unity 프로젝트 구조와 씬 구성을 실험하는 저장소입니다. 현재는 Unity 기본 프로젝트와 에셋 구성이 중심이며, 기능 확장을 위한 초기 상태로 볼 수 있습니다.

## 기술 스택

- Unity `6000.0.41f1`
- C#
- Unity Scene 시스템
- Unity Package Manager

## 현재 구성

- `Assets/`: 씬, 설정, 에셋 파일
- `Packages/`: Unity 패키지 의존성
- `ProjectSettings/`: Unity 프로젝트 설정
- `Assets/Settings/Project Configuration/SceneTemplate_RotateCube.cs`: 프로젝트 설정 예시 스크립트

## 폴더 구조

```text
.
├── Assets/
├── Packages/
├── ProjectSettings/
└── README.md
```

## 실행 방법

1. Unity Hub에서 `6000.0.41f1` 버전을 설치합니다.
2. 저장소 루트 폴더를 Unity 프로젝트로 엽니다.
3. 패키지 복원이 끝날 때까지 기다립니다.
4. `Assets` 아래의 씬을 열고 Play 버튼으로 실행합니다.

## 개발 방향 제안

이 저장소를 계속 확장한다면 다음 항목을 README와 함께 정리하는 것을 권장합니다.

- 프로젝트 목표
- 시작 씬 이름
- 조작법
- 구현된 기능 목록
- 직접 작성한 스크립트 목록
- 빌드 파일 또는 플레이 영상 링크

## 현재 상태

현재 README는 Unity 프로젝트의 기본 진입 문서입니다. 기능이 추가될수록 위 항목을 구체화하면 프로젝트 이해도가 크게 좋아집니다.
