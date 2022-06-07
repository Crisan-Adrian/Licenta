import json


def convert_data(fileName):
    f = open(fileName)

    _data = json.load(f)
    f.close()

    frames = _data["frames"]

    data = []

    for frame in frames:
        dataRow = []
        for x in frame:
            dataRow.append(frame[x])
        data.append(dataRow)

    return data

if __name__ == '__main__':
    convert_data('../datasets/Data.json')
